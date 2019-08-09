using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [Serializable]
    public class Wave
    {
        public List<EnemyCategory> Enemies;
        public Vector2 TimeRangeBetweenSpawns;
        public float TimeUntilNext;
    }

    [Header("Stats")]
    public List<Wave> Waves;

    public float StartGameReadyTime;

    public Vector2Bag ButterflySpawnLocations, HamsterSpawnLocations;
    public Vector2Int HamsterHordeSizeRange;

    [Range(0, 1)]
    public float GooseTargetsActiveChance;

    [Header("References")]
    public Butterfly ButterflyPrefab;
    public Hamster HamsterPrefab;
    public Goose GoosePrefab;

    public int CurrentWave { get; private set; }

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
        yield return new WaitForSeconds(StartGameReadyTime);

        foreach (var wave in Waves)
        {
            CurrentWave++;
            StartCoroutine(waveRoutine(wave));

            float timer = 0;
            while (timer < wave.TimeUntilNext && BaseEnemy.TotalEnemies != 0)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        yield return new WaitUntil(() => BaseEnemy.TotalEnemies == 0);

        // TODO: victory screen
        Debug.Log("you win");
    }

    IEnumerator waveRoutine (Wave wave)
    {
        var copyList = new List<EnemyCategory>(wave.Enemies);
        copyList.ShuffleInPlace();

        foreach (var enemy in copyList)
        {
            spawnEnemy(enemy);
            yield return new WaitForSeconds(RandomExtra.Range(wave.TimeRangeBetweenSpawns));
        }
    }

    void spawnEnemy (EnemyCategory enemy)
    {
        switch (enemy)
        {
            case EnemyCategory.Hamster:
                spawnHamsterHorde();
                break;

            case EnemyCategory.Goose:
                spawnGoose();
                break;

            case EnemyCategory.Butterfly:
                spawnButterfly();
                break;
        }
    }

    void spawnHamsterHorde ()
    {
        int limit = RandomExtra.Range(HamsterHordeSizeRange);
        for (int i = 0; i < limit; i++)
        {
            Instantiate(HamsterPrefab);
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

    void spawnButterfly ()
    {
        Instantiate(ButterflyPrefab).Initialize((MagicColor) UnityEngine.Random.Range(0, 3));
    }
}

public enum EnemyCategory
{
    Hamster, Goose, Butterfly
}

[Serializable]
public class Vector2Bag : BagRandomizer<Vector2> {}
