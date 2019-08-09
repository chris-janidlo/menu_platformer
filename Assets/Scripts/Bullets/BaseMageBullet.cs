using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseMageBullet : MonoBehaviour
{
    public const float ExtraEffectChance = 0.1f;
    // red effect
    public const float FireDamagePerSecond = 10;
    public const float FireTimeMin = 1, FireTimMax = 5;
    // green effect
    public const float HealAmount = 10;
    // blue effect
    public const float IceSlowPercent = .8f;
    public const float IceTimeMin = 2, IceTimeMax = 7;

    public MagicColor Color => Visuals.Color;

    [Header("Stats")]
    [Header("Base")]
    public float Damage;

    [Header("References")]
    public ColorMapApplier Visuals;

    bool appliedExtraEffect;
    protected Rigidbody2D rb;

    public virtual void Initialize (MagicColor color)
    {
        Visuals.Color = color;
        rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        bool effect = false;
        if (!appliedExtraEffect && RandomExtra.Chance(ExtraEffectChance))
        {
            appliedExtraEffect = false;
            effect = true;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (effect && Color == MagicColor.Green)
            {
                other.GetComponent<Mage>().Health.Heal(HealAmount);
            }
            return;
        }

        BaseEnemy enemy = other.gameObject.GetComponent<BaseEnemy>();

        if (enemy != null)
        {
            enemy.Health.ColorDamage(Damage, Color);

            if (effect)
            {
                if (Color == MagicColor.Red)
                {
                    enemy.ApplyFire(Random.Range(FireTimeMin, FireTimMax));
                }

                if (Color == MagicColor.Blue)
                {
                    enemy.ApplyIce(Random.Range(IceTimeMin, IceTimeMax));
                }
            }
        }

        Destroy(gameObject);
    }
}
