using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BurstBullet : MonoBehaviour
{
    public MagicColor Color => Visuals.Color;

    [Header("Stats")]
    public SpellPowerContainer Speeds;
    public AnimationCurve ShrinkCurve;
    public SpellPowerContainer Damages, ScaleMultipliers;

    [Header("References")]
    public ColorMapApplier Visuals;

    SpellPower power;
    float lifeTimer;
    Rigidbody2D rb;

    public void Initialize (Vector2 direction, MagicColor color, SpellPower power)
    {
        Visuals.Color = color;
        this.power = power;

        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * Speeds[power];
    }

    void Update ()
    {
        float scale = ShrinkCurve.Evaluate(lifeTimer) * ScaleMultipliers[power];

        if (scale <= 0)
        {
            Destroy(gameObject);
            return;
        }

        transform.localScale = new Vector3(scale, scale, scale);

        lifeTimer += Time.deltaTime;
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
}
