using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    public abstract float MaxHealth { get; }

    bool _healthInit;
    [SerializeField]
    float _health;
    public float Health
    {
        get
        {
            if (!_healthInit)
            {
                _health = MaxHealth;
                _healthInit = true;
            }

            return _health;
        }
        set
        {
            if (!_healthInit) _healthInit = true;
            _health = Mathf.Clamp(value, 0, MaxHealth);
            if (_health == 0) die();
        }
    }

    float fireTimer;

    protected void Update ()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer > 0) Health -= BaseMageBullet.FireDamagePerSecond * Time.deltaTime;
    }

    public void ApplyFire (float fireTime)
    {
        fireTimer = fireTime;

        // TODO: spawn fire effect
    }

    public void ApplyIce (float slowTime)
    {
        // TODO: spawn ice effect

        iceImpl(slowTime);
    }

    void die ()
    {
        // TODO: spawn death effect
    }

    protected abstract void iceImpl (float slowTime);
}
