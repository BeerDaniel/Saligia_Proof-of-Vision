using SaligiaProofOfVision;
using System;
using UnityEngine;

public class Player : BaseUnit
{
    [field: SerializeField]
    public float MaxMana { get; private set; }
    public float CurrentMana { get; private set; }

    [Header("Components")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Animator _animator;
    public Animator Animator => _animator;
    public InputReader InputReader => _inputReader;

    [Header("Misc")]
    [SerializeField] private Transform _spawnPos;

    protected override void Start()
    {
        base.Start();
        CurrentMana = MaxMana;

        GameEvents.healthChangedEvent?.Invoke(CurrentHealth, MaxHealth);
        GameEvents.manaChangedEvent?.Invoke(CurrentMana, MaxMana);
    }

    private void OnEnable()
    {
        SubscribeToInputEvents(true);
    }

    private void OnDisable()
    {
        SubscribeToInputEvents(false);
    }

    private void SubscribeToInputEvents(bool value)
    {
        if (value)
        {
            GameEvents.resetGameEvent += OnResetGame;
        }
        else
        {
            GameEvents.resetGameEvent -= OnResetGame;
        }
    }

    public override void ApplyDamage(DamageMessage dmgMessage, Action<EnemyUnit> killCallback = null)
    {
#if DEBUG
        if (IsInvulnerable)
            return;
#endif
        CurrentHealth = Mathf.Clamp(CurrentHealth - dmgMessage.damage, 0, maxHealth);
        GameEvents.healthChangedEvent?.Invoke(CurrentHealth, MaxHealth);
        if (CurrentHealth <= 0)
        {
            _animator.SetTrigger("die");
            GameEvents.playerDeathEvent?.Invoke();
        }
    }

    public void RecudeMana(float value)
    {
#if DEBUG
        if (IsInvulnerable)
            return;
#endif
        CurrentMana = Mathf.Clamp(CurrentMana - value, 0, MaxMana);
        GameEvents.manaChangedEvent?.Invoke(CurrentMana, MaxMana);
    }

    public void RestoreRessources()
    {
        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;
        GameEvents.healthChangedEvent?.Invoke(CurrentHealth, MaxHealth);
        GameEvents.manaChangedEvent?.Invoke(CurrentMana, MaxMana);
    }

    public void OnResetGame()
    {
        _animator.SetTrigger("retry");
        RestoreRessources();
        if (_spawnPos != null)
            transform.position = _spawnPos.position;
    }
}
