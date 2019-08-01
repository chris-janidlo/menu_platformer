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
    public SpellPowerContainer Damages;

    [Header("References")]
    public ColorMapApplier Visuals;

    bool appliedExtraEffect;
    protected SpellPower power;
    protected Rigidbody2D rb;

    public virtual void Initialize (MagicColor color, SpellPower power)
    {
        Visuals.Color = color;
        this.power = power;

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

        Debug.Log(LayerMask.LayerToName(other.gameObject.layer));
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (effect && Color == MagicColor.Green)
            {
                other.GetComponent<Mage>().Health += HealAmount;
            }
            return;
        }

        BaseEnemy enemy = other.gameObject.GetComponent<BaseEnemy>();

        if (enemy != null)
        {
            enemy.Health -= Damages[power];

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

    protected void setScale (SpellPowerContainer scales)
    {
        var scale = scales[power];
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
