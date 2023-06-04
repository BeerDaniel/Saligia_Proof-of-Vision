using SaligiaProofOfVision;
using SaligiaProofOfVision.Abilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject _abilityInfoOverlay;
    [SerializeField] private GameObject _gameOverOverlay;

    [Space]
    [SerializeField] private List<TextMeshProUGUI> _keyLabels;
    [SerializeField] private TextMeshProUGUI _t1Label;
    [SerializeField] private List<GameObject> _t1Buttons;
    [SerializeField] private TextMeshProUGUI _t2Label;
    [SerializeField] private List<GameObject> _t2Buttons;

    [Space]
    [SerializeField] private TextMeshProUGUI _gameOverLabel;

    [Header("Misc")]
    [SerializeField] private InputReader _inputReader;

    public static int NumOverlaysActive { get; private set; } = 0;
    public bool GameOverOverlayActive { get; private set; } = false;

    #region Scaling Fields
    public static bool AbilityOverlayActive { get; private set; } = false;

    private int _runeTier = 0;
    #endregion

    private void Start()
    {
        _t1Label.enabled = false;
        _t2Label.enabled = false;
        for (int i = 0; i < _t1Buttons.Count; i++)
        {
            AbilitySlot slot = (AbilitySlot)i;
            _t1Buttons[i].GetComponent<Button>().onClick.AddListener(delegate { OnUpgradeAbility(slot, true); });
            _t2Buttons[i].GetComponent<Button>().onClick.AddListener(delegate { OnUpgradeAbility(slot, false); });
            _t1Buttons[i].SetActive(false);
            _t2Buttons[i].SetActive(false);
        }
        _abilityInfoOverlay.SetActive(false);

        _gameOverLabel.text = "Menü";
        _gameOverOverlay.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.controlsChangedEvent += OnControlsChanged;
        GameEvents.playerDeathEvent += OnPlayerDeath;
        GameEvents.menuEvent += OnMenuEvent;
        GameEvents.abilityOverlayEvent += OnAbilityOverlayEvent;
        GameEvents.bossEnemyDeathEvent += OnBossEnemyDeath;
    }

    private void OnDisable()
    {
        GameEvents.controlsChangedEvent -= OnControlsChanged;
        GameEvents.playerDeathEvent -= OnPlayerDeath;
        GameEvents.menuEvent -= OnMenuEvent;
        GameEvents.abilityOverlayEvent -= OnAbilityOverlayEvent;
        GameEvents.bossEnemyDeathEvent -= OnBossEnemyDeath;
    }

    #region Event Methods
    private void OnPlayerDeath()
    {
        _gameOverLabel.text = "Game Over";
        if (!GameOverOverlayActive)
            ToggleGameOverOverlay();
        if (NumOverlaysActive > 0)
            _inputReader.EnableUIInput();
        else
            _inputReader.EnableGameplayInput();
    }

    private void OnControlsChanged(string currentControlScheme)
    {
        _keyLabels[0].text = _inputReader.GetBindingStringForAction(_inputReader.Controls.Player.PrimaryAbility);
        _keyLabels[1].text = _inputReader.GetBindingStringForAction(_inputReader.Controls.Player.SecondaryAbility1);
        _keyLabels[2].text = _inputReader.GetBindingStringForAction(_inputReader.Controls.Player.SecondaryAbility2);
        _keyLabels[3].text = _inputReader.GetBindingStringForAction(_inputReader.Controls.Player.MovementAbility);
    }

    private void OnAbilityOverlayEvent()
    {
        ToggleAbilityOverlay();
    }

    private void OnMenuEvent()
    {
        ToggleGameOverOverlay();
    }

    private void OnBossEnemyDeath(EnemyUnit enemy)
    {
        if (!AbilityOverlayActive)
            ToggleAbilityOverlay();
        _runeTier++;
        ShowRunes();
    }
    #endregion

    #region ButtonCallbacks
    private void OnUpgradeAbility(AbilitySlot abilitySlot, bool isT1)
    {
        int slot = (int)abilitySlot;
        List<GameObject> buttons;
        if (isT1)
            buttons = _t1Buttons;
        else
            buttons = _t2Buttons;

        for (int i = 0; i < buttons.Count; i++)
        {
            if (i == slot)
            {
                buttons[i].GetComponent<Outline>().effectColor = Color.green;
                buttons[i].GetComponentInChildren<Text>().color = Color.white;
            }
            else
            {
                buttons[i].GetComponent<Outline>().effectColor = Color.grey;
                buttons[i].GetComponentInChildren<Text>().color = Color.grey;
            }
        }
        GameEvents.upgradeAbilityEvent?.Invoke(abilitySlot, isT1);
        
        //if (isT1)
        //{
        //    for (int i = 0; i < _t1Buttons.Count; i++)
        //    {
        //        if (i == slot)
        //        {
        //            _t1Buttons[i].GetComponent<Outline>().effectColor = Color.green;
        //            _t1Buttons[i].GetComponentInChildren<Text>().color = Color.white;
        //        }
        //        else
        //        {
        //            _t1Buttons[i].GetComponent<Outline>().effectColor = Color.grey;
        //            _t1Buttons[i].GetComponentInChildren<Text>().color = Color.grey;
        //        }
        //    }
        //    GameEvents.upgradeAbilityEvent?.Invoke(abilitySlot, isT1);
        //}
        //else
        //{
        //    for (int i = 0; i < _t2Buttons.Count; i++)
        //    {
        //        if (i == slot)
        //        {
        //            _t2Buttons[i].GetComponent<Outline>().effectColor = Color.green;
        //        }
        //        else
        //        {
        //            _t2Buttons[i].GetComponent<Outline>().effectColor = Color.grey;
        //            _t2Buttons[i].GetComponentInChildren<Text>().color = Color.grey;
        //        }
        //    }
        //    GameEvents.upgradeAbilityEvent?.Invoke(abilitySlot, isT1);
        //}
    }

    public void OnResetButton()
    {
        //TODO: Remove this fix after prototype for FFF
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        GameEvents.resetGameEvent?.Invoke();
        ResetAbilityOverlay();
        _gameOverLabel.text = "Menü";
        _runeTier = 0;
        if (GameOverOverlayActive)
            ToggleGameOverOverlay();
        if (AbilityOverlayActive)
            ToggleAbilityOverlay();
    }

    public void OnCloseButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
    #endregion

    private void ToggleAbilityOverlay()
    {
        if (AbilityOverlayActive)
        {
            AbilityOverlayActive = false;
            NumOverlaysActive--;
        }
        else
        {
            AbilityOverlayActive = true;
            NumOverlaysActive++;
        }
        if (NumOverlaysActive == 0)
        {
            Time.timeScale = 1;
            _inputReader.EnableGameplayInput();
        }
        else
        {
            _inputReader.EnableUIInput();
            Time.timeScale = 0;
        }

        _abilityInfoOverlay.SetActive(AbilityOverlayActive);
    }

    private void ToggleGameOverOverlay()
    {
        GameOverOverlayActive = !GameOverOverlayActive;
        if (GameOverOverlayActive)
        {
            NumOverlaysActive++;
            _inputReader.EnableUIInput();
            Time.timeScale = 0;
        }
        else
        {
            NumOverlaysActive--;
            if (NumOverlaysActive == 0)
            {
                Time.timeScale = 1;
                _inputReader.EnableGameplayInput();
            }
        }
        _gameOverOverlay.SetActive(GameOverOverlayActive);
    }

    private void ResetAbilityOverlay()
    {

        for (int i = 0; i < _t2Buttons.Count; i++)
        {
            _t1Buttons[i].GetComponent<Outline>().effectColor = Color.white;
            _t1Buttons[i].GetComponentInChildren<Text>().color = Color.white;
            _t2Buttons[i].GetComponent<Outline>().effectColor = Color.white;
            _t2Buttons[i].GetComponentInChildren<Text>().color = Color.white;

            _t1Buttons[i].SetActive(false);
            _t2Buttons[i].SetActive(false);
        }

        _t1Label.enabled = false;
        _t2Label.enabled = false;

        if (_abilityInfoOverlay.activeSelf)
            ToggleAbilityOverlay();
    }

    private void ShowRunes()
    {
        if (_runeTier == 1)
        {
            _t1Label.enabled = true;
            foreach (var button in _t1Buttons)
                button.SetActive(true);
        }
        if (_runeTier == 2)
        {
            _t2Label.enabled = true;
            foreach (var button in _t2Buttons)
                button.SetActive(true);
        }
    }
}
