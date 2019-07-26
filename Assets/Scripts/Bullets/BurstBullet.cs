using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstBullet : BaseMageBullet
{
    [Header("Stats")]
    [Header("Burst")]
    public SpellPowerContainer Speeds;
    public AnimationCurve ShrinkCurve;
    public SpellPowerContainer ScaleMultipliers;

    float lifeTimer;

    public void Initialize (Vector2 direction, MagicColor color, SpellPower power)
    {
        base.Initialize(color, power);

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
}
