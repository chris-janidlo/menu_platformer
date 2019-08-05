using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColoredHealth : MonoBehaviour
{
    public bool IsColored = true; // for goose
    public MagicColor Color;
    public float MaxHealth;

    public int HurtFlashes = 4;
    public float HurtFlashOnTime = .1f, HurtFlashOffTime = .1f;

    public UnityEvent Death, Revival;

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
            var beforeHealth = _currentHealth;

            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);

            if (!dead && _currentHealth == 0)
            {
                dead = true;
                Death.Invoke();
            }

            if (dead && _currentHealth != 0)
            {
                dead = false;
                Revival.Invoke();
            }

            if (_currentHealth != 0 && value != 0 && value < beforeHealth)
            {
                if (!flashing) StartCoroutine(hurtFlashRoutine());
            }
        }
    }

    public bool Dead => CurrentHealth == 0;

    bool dead, flashing;

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

    IEnumerator hurtFlashRoutine ()
    {
        flashing = true;

        var srs = GetComponentsInChildren<SpriteRenderer>();

        Action<bool> setVisible = v =>
        {
            foreach (var sr in srs)
            {
                sr.SetAlpha(v ? 1 : 0);
            }
        };

        for (int i = 0; i < HurtFlashes; i++)
        {
            setVisible(false);
            yield return new WaitForSeconds(HurtFlashOffTime);

            setVisible(true);
            yield return new WaitForSeconds(HurtFlashOnTime);
        }

        flashing = false;
    }
}
