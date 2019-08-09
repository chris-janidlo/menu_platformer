using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

[RequireComponent(typeof(ColoredHealth))]
public abstract class BaseEnemy : MonoBehaviour
{
    public const float ItemDropRate = .1f;
    public const float HealthPotChance = .4f;

    public static int TotalEnemies { get; private set; }

    public ColoredHealth Health;
    public ElementalParticleEffect ElementalParticleEffectPrefab;
    public Item HealthPotPrefab, ManaPotPrefab;

    protected bool isFrozen => iceTimer >= 0;

    float fireTimer, iceTimer;
    ElementalParticleEffect fireEffect, iceEffect;

    protected virtual void Awake ()
    {
        TotalEnemies++;

        Health.Death.AddListener(() => {
            TotalEnemies--;

            if (RandomExtra.Chance(ItemDropRate))
            {
                Instantiate(RandomExtra.Chance(HealthPotChance) ? HealthPotPrefab : ManaPotPrefab, transform.position, Quaternion.identity);
            }

            die();
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

    protected abstract void die ();
}
