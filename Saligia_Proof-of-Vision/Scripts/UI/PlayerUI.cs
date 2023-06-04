using SaligiaProofOfVision.Abilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;

    #region UI Components
    [Header("Player Bars")]
    [SerializeField] private float _barAnimationSpeed;
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _healthBarAnimation;
    [SerializeField] private Image _manaBar;
    [SerializeField] private Image _manaBarAnimation;
    [Header("Ability Labels")]
    [SerializeField] private TextMeshProUGUI _primaryAbilityLabel;
    [SerializeField] private TextMeshProUGUI _secondary1AbilityLabel;
    [SerializeField] private TextMeshProUGUI _secondary2AbilityLabel;
    [SerializeField] private TextMeshProUGUI _movementAbilityLabel;
    [SerializeField] private TextMeshProUGUI _keyinfoLabel;
    [Header("Ability Cooldowns")]
    [SerializeField] private List<Image> _cooldownImages;
    [SerializeField] private List<TextMeshProUGUI> _cooldownTexts;
    #endregion    

    private Coroutine _healthBarRoutine;
    private Coroutine _manaBarRoutine;

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            _cooldownImages[i].enabled = false;
            _cooldownTexts[i].enabled = false;
        }
    }

    private void OnEnable()
    {
        GameEvents.healthChangedEvent += OnPlayerHealthChanged;
        GameEvents.manaChangedEvent += OnPlayerManaChanged;
        GameEvents.cooldownStartEvent += OnCooldownStart;
        GameEvents.cooldownUpdateEvent += OnCooldownUpdate;
        GameEvents.cooldownEndEvent += OnCooldownEnd;
        GameEvents.controlsChangedEvent += OnControlsChanged;
    }

    private void OnDisable()
    {
        GameEvents.healthChangedEvent -= OnPlayerHealthChanged;
        GameEvents.manaChangedEvent -= OnPlayerManaChanged;
        GameEvents.cooldownStartEvent -= OnCooldownStart;
        GameEvents.cooldownUpdateEvent -= OnCooldownUpdate;
        GameEvents.cooldownEndEvent -= OnCooldownEnd;
        GameEvents.controlsChangedEvent -= OnControlsChanged;
    }

    private void OnControlsChanged(string currentControlScheme)
    {
        string menuString = "Menü: <menukey>\nInfo: <infokey>";
        menuString = menuString.Replace("<menukey>", _inputReader.GetBindingStringForAction(_inputReader.Controls.Overlay.GameOverMenu));
        menuString = menuString.Replace("<infokey>", _inputReader.GetBindingStringForAction(_inputReader.Controls.Overlay.ScalingMenu));
        _keyinfoLabel.text = menuString;

        _primaryAbilityLabel.text = _inputReader.GetBindingStringForAction(_inputReader.Controls.Player.PrimaryAbility);
        _secondary1AbilityLabel.text = _inputReader.GetBindingStringForAction(_inputReader.Controls.Player.SecondaryAbility1);
        _secondary2AbilityLabel.text = _inputReader.GetBindingStringForAction(_inputReader.Controls.Player.SecondaryAbility2);
        _movementAbilityLabel.text = _inputReader.GetBindingStringForAction(_inputReader.Controls.Player.MovementAbility);
    }

    private void OnPlayerHealthChanged(float currentHealth, float maxHealth)
    {
        _healthBar.fillAmount = currentHealth / maxHealth;
        if (_healthBarRoutine == null)
            _healthBarRoutine = StartCoroutine(AnimateHealthBar(_healthBar.fillAmount));
        else
        {
            StopCoroutine(_healthBarRoutine);
            _healthBarRoutine = StartCoroutine(AnimateHealthBar(_healthBar.fillAmount));
        }
    }

    private IEnumerator AnimateHealthBar(float endValue)
    {
        float time = 0;

        while (_healthBarAnimation.fillAmount > endValue)
        {
            _healthBarAnimation.fillAmount = Mathf.Lerp(_healthBarAnimation.fillAmount, endValue, time * _barAnimationSpeed);
            time += Time.deltaTime;
            yield return null;
        }
        _healthBarAnimation.fillAmount = endValue;
        _healthBarRoutine = null;
    }

    private void OnPlayerManaChanged(float currentMana, float maxMana)
    {
        _manaBar.fillAmount = currentMana / maxMana;
        if (_manaBarRoutine == null)
            _manaBarRoutine = StartCoroutine(AnimateManaBar(_manaBar.fillAmount));
        else
        {
            StopCoroutine(_manaBarRoutine);
            _manaBarRoutine = StartCoroutine(AnimateManaBar(_manaBar.fillAmount));
        }
    }

    private IEnumerator AnimateManaBar(float endValue)
    {
        float time = 0;

        while (_manaBarAnimation.fillAmount > endValue)
        {
            _manaBarAnimation.fillAmount = Mathf.Lerp(_manaBarAnimation.fillAmount, endValue, time * _barAnimationSpeed);
            time += Time.deltaTime;
            yield return null;
        }
        _manaBarAnimation.fillAmount = endValue;
        _manaBarRoutine = null;
    }

    private void OnCooldownStart(int index, float value)
    {
        var cdImage = _cooldownImages[index];
        var cdText = _cooldownTexts[index];
        cdImage.enabled = true;
        cdText.enabled = true;
        cdImage.fillAmount = 1;
        cdText.text = ((int)value).ToString();
    }

    private void OnCooldownUpdate(int index, float value, float baseValue)
    {
        var cdImage = _cooldownImages[index];
        var cdText = _cooldownTexts[index];
        cdImage.fillAmount = value / baseValue;
        cdText.text = ((int)value).ToString();
    }

    private void OnCooldownEnd(int index)
    {
        if (_cooldownImages[index].enabled)
            _cooldownImages[index].enabled = false;
        if (_cooldownTexts[index].enabled)
            _cooldownTexts[index].enabled = false;
    }
}
