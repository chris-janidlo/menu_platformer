using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseMageBullet : MonoBehaviour
{
    public MagicColor Color => Visuals.Color;

    [Header("Stats")]
    [Header("Base")]
    public SpellPowerContainer Damages;

    [Header("References")]
    public ColorMapApplier Visuals;

    protected SpellPower power;
    protected Rigidbody2D rb;

    public virtual void Initialize (MagicColor color, SpellPower power)
    {
        Visuals.Color = color;
        this.power = power;

        rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // TODO: green effect
            return;
        }

        BaseEnemy enemy = other.gameObject.GetComponent<BaseEnemy>();

        if (enemy != null)
        {
            // TODO: damage
        }

        Destroy(gameObject);
    }

    protected void setScale (SpellPowerContainer scales)
    {
        var scale = scales[power];
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
