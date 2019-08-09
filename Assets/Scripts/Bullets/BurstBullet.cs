using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstBullet : BaseMageBullet
{
    [Header("Stats")]
    [Header("Burst")]
    public float Speed;
    public AnimationCurve ShrinkCurve;

    float lifeTimer;

    public void Initialize (Vector2 direction, MagicColor color)
    {
        base.Initialize(color);

        rb.velocity = direction.normalized * Speed;
    }

    void Update ()
    {
        float scale = ShrinkCurve.Evaluate(lifeTimer);

        if (scale <= 0)
        {
            Destroy(gameObject);
            return;
        }

        transform.localScale = new Vector3(scale, scale, scale);

        lifeTimer += Time.deltaTime;
    }
}
