using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class HamsterFart : MonoBehaviour
{
    public float DamagePerSecond;
    public int Clouds;
    public Vector2 CloudSpeedRange;
    public HamsterFartCloud FartCloudPrefab;

    MagicColor color;

    // we use this list for two reasons
        // for any individual mage, we don't want to hurt it n times if it's touching n clouds. this means that instead of each cloud dealing damage, we need the parent fart to deal the damage, and so we need farts to track which mage they're damaging
        // we want to hurt more than one mage if more than one is touching any cloud in the fart. this means we can't just keep track of one mage
    // child clouds are responsible for calling SetMageStatus so that this knows who to damage
    List<Mage> currentlyDamaging = new List<Mage>();

    public void Initialize (MagicColor color)
    {
        this.color = color;

        for (int i = 0; i < Clouds; i++)
        {
            spawnCloud();
        }
    }

    void Update ()
    {
        if (GetComponentsInChildren<HamsterFartCloud>().Length == 0)
        {
            Destroy(gameObject);
        }

        foreach (var mage in currentlyDamaging)
        {
            mage.Health.ColorDamage(DamagePerSecond * Time.deltaTime, color);
        }
    }

    public void SetMageStatus (Mage mage, bool shouldDamage)
    {
        if (shouldDamage && !currentlyDamaging.Contains(mage))
        {
            currentlyDamaging.Add(mage);
            return;
        }

        if (!shouldDamage && currentlyDamaging.Contains(mage))
        {
            currentlyDamaging.Remove(mage);
            return;
        }
    }

    void spawnCloud ()
    {
        var velocity = Random.insideUnitCircle.normalized * RandomExtra.Range(CloudSpeedRange);

        var angles = new List<Quaternion>()
        {
            Quaternion.Euler(0, 0, 0),
            Quaternion.Euler(0, 0, 90),
            Quaternion.Euler(0, 0, 180),
            Quaternion.Euler(0, 0, 270),
        };

        Instantiate(FartCloudPrefab, transform.position, angles.PickRandom(), transform).Initialize(this, velocity, color);
    }
}
