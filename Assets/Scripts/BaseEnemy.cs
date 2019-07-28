using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    public const float MaxHealth = 100;

    [SerializeField]
    float _health = MaxHealth;
    public float Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);
            if (_health == 0) die();
        }
    }

    protected abstract void die ();

    public abstract void ApplyRedBullet ();
    public abstract void ApplyBlueBullet ();
}
