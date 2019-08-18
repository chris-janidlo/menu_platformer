using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [Header("Stats")]
    public int CurrentWave;
    public int MaxWave;
    public EnemyBag Enemies;
    public AnimationCurve SpawnTimeByEnemiesSpawned;

    public float StartGameReadyTime;

    public TransformBag ButterflySpawnLocations, HamsterSpawnLocations;

    [Range(0, 1)]
    public float GooseTargetsActiveChance;

    [Header("References")]
    public Butterfly ButterflyPrefab;
    public Hamster HamsterPrefab;
    public Goose GoosePrefab;

    List<Mage> currentGooseTargets = new List<Mage>();

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }

    public void StartGame ()
    {
        StartCoroutine(gameRoutine());
    }

    IEnumerator gameRoutine ()
    {
        CurrentWave = 0;

        yield return new WaitForSeconds(StartGameReadyTime);

        int enemies = 0;

        while (CurrentWave <= MaxWave)
        {
            if (enemies % Enemies.Items.Count == 0) CurrentWave++;

            spawnEnemy(Enemies.GetNext());

            yield return new WaitForSeconds(SpawnTimeByEnemiesSpawned.Evaluate(enemies));

            enemies++;
        }

        yield return new WaitUntil(() => BaseEnemy.TotalEnemies == 0);

        EndScreen.Victory.StartSequence();
    }

    void spawnEnemy (EnemyCategory enemy)
    {
        switch (enemy)
        {
            case EnemyCategory.Hamster:
                Instantiate(HamsterPrefab);
                break;

            case EnemyCategory.Butterfly:
                Instantiate(ButterflyPrefab);
                break;

            case EnemyCategory.Goose:
                spawnGoose();
                break;

            default:
                throw new ArgumentException($"unexpected enemy type {enemy}");
        }
    }

    void spawnGoose ()
    {
        Mage target;

        if (RandomExtra.Chance(GooseTargetsActiveChance) && !currentGooseTargets.Contains(MageSquad.Instance.ActiveMage))
        {
            target = MageSquad.Instance.ActiveMage;
        }
        else
        {
            var options = new List<Mage>
            {
                MageSquad.Instance.RedMage,
                MageSquad.Instance.GreenMage,
                MageSquad.Instance.BlueMage
            }
            .Where(m => !m.Active && !currentGooseTargets.Contains(m))
            .ToList();

            if (options.Count == 0) return;

            target = options.PickRandom();
        }

        currentGooseTargets.Add(target);

        var goose = Instantiate(GoosePrefab);
        goose.Initialize(target.transform);
        goose.Health.Death.AddListener(() => currentGooseTargets.Remove(target));
    }
}

public enum EnemyCategory
{
    Hamster, Goose, Butterfly
}

[Serializable]
public class TransformBag : BagRandomizer<Transform> {}

[Serializable]
public class EnemyBag : BagRandomizer<EnemyCategory> {}
