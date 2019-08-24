using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

[RequireComponent(typeof(ColoredHealth))]
public abstract class BaseEnemy : MonoBehaviour
{
    public static int TotalEnemies { get; private set; }

    public ColoredHealth Health;
    public ElementalParticleEffect ElementalParticleEffectPrefab;

    protected bool isFrozen => iceTimer >= 0;

    float fireTimer, iceTimer;
    ElementalParticleEffect fireEffect, iceEffect;

    protected virtual void Awake ()
    {
        TotalEnemies++;

        Health.Death.AddListener(() => {
            TotalEnemies--;

            if (RandomExtra.Chance(EnemySpawner.Instance.ItemDropRate))
            {
                Instantiate(EnemySpawner.Instance.ItemDropDistribution.GetNext(), transform.position, Quaternion.identity);
            }
        });
    }

    protected virtual void Update ()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer > 0)
        {
            Health.ColorDamage(BaseMageBullet.FireDamagePerSecond * Time.deltaTime, MagicColor.Red);
        }
        else if (fireEffect != null)
        {
            Destroy(fireEffect.gameObject);
        }

        iceTimer -= Time.deltaTime;

        if (iceTimer <= 0 && iceEffect != null)
        {
            Destroy(iceEffect.gameObject);
        }
    }

    public void ApplyFire (float fireTime)
    {
        fireTimer = fireTime;

        fireEffect = Instantiate(ElementalParticleEffectPrefab, transform);
        fireEffect.SetColor(MagicColor.Red);
    }

    public void ApplyIce (float slowTime)
    {
        iceTimer = slowTime;

        iceEffect = Instantiate(ElementalParticleEffectPrefab, transform);
        iceEffect.SetColor(MagicColor.Blue);
    }
}
