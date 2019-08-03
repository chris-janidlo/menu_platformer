using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ColoredHealth))]
public abstract class BaseEnemy : MonoBehaviour
{
    public ColoredHealth Health;

    protected bool isFrozen => iceTimer >= 0;

    float fireTimer, iceTimer;

    public abstract void Initialize (MagicColor color);

    protected virtual void Awake ()
    {
        Health.Death.AddListener(die);
    }

    protected virtual void Update ()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer > 0)
        {
            Health.ColorDamage(BaseMageBullet.FireDamagePerSecond * Time.deltaTime, MagicColor.Red);
        }

        iceTimer -= Time.deltaTime;
    }

    public void ApplyFire (float fireTime)
    {
        fireTimer = fireTime;

        // TODO: spawn fire effect
    }

    public void ApplyIce (float slowTime)
    {
        iceTimer = slowTime;

        // TODO: spawn ice effect
    }

    protected abstract void die ();
}
