using System;
using UnityEngine;

namespace SaligiaProofOfVision
{
    public abstract class BaseUnit : MonoBehaviour, IDamageable
    {
        public float MaxHealth => maxHealth;
        [SerializeField] protected float maxHealth;
        public float CurrentHealth { get; protected set; }

        [field: SerializeField]
        public bool IsInvulnerable { get; protected set; }

        protected virtual void Start()
        {
            CurrentHealth = MaxHealth;
        }

        public virtual void ResetDamage()
        {
            CurrentHealth = MaxHealth;
        }

        public abstract void ApplyDamage(DamageMessage dmgMessage, Action<EnemyUnit> killCallback = null);
    }
}