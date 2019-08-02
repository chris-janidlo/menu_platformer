using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColoredHealth : MonoBehaviour
{
    public bool IsColored = true; // for goose
    public MagicColor Color;
    public float MaxHealth;
    public UnityEvent Death, Revival;

    bool dead;

    bool _healthInit;
    [SerializeField]
    float _currentHealth;
    public float CurrentHealth
    {
        get
        {
            if (!_healthInit)
            {
                _healthInit = true;
                _currentHealth = MaxHealth;
            }

            return _currentHealth;
        }

        private set
        {
            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);

            if (MaxHealth == 0)
            {
                dead = true;
                Death.Invoke();
            }
            else if (dead)
            {
                dead = false;
                Revival.Invoke();
            }
        }
    }

    public bool Dead => CurrentHealth == 0;

    public void ColorDamage (float damage, MagicColor color)
    {
        if (!IsColored)
        {
            PureDamage(damage);
            return;
        }

        float dam = damage;
    
        switch (color.Compare(Color))
        {
            case -1:
                dam *= MagicColorStats.WeakDamage;
                break;

            case 1:
                dam *= MagicColorStats.SuperEffectiveDamage;
                break;
        }

        CurrentHealth -= dam;
    }

    public void PureDamage (float damage)
    {
        CurrentHealth -= damage;
    }

    public void Heal (float amount)
    {
        CurrentHealth += amount;
    }

    public void FullHeal ()
    {
        CurrentHealth = MaxHealth;
    }
}
