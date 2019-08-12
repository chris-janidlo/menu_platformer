using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [Serializable]
    public class EnemyPack
    {
        public EnemyCategory Category;
        public int Number = 1;
    }

    [Serializable]
    public class Wave
    {
        public List<EnemyPack> Packs;
        public float TimeUntilFirstSpawm;
        public Vector2 TimeRangeBetweenSpawns;
        public float MaxTimeUntilNextWave;
    }

    [Header("Stats")]
    public int CurrentWave;
    public List<Wave> Waves;

    public float StartGameReadyTime;

    public TransformBag ButterflySpawnLocations, HamsterSpawnLocations;

    [Range(0, 1)]
    public float GooseTargetsActiveChance;

    [Header("References")]
    public Butterfly ButterflyPrefab;
    public Hamster HamsterPrefab;
    public Goose GoosePrefab;

    List<Mage> currentGooseTargets = new List<Mage>();
    bool currentWaveDefeated;
    IEnumerator currentWaveEnum;

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

        // allow the game to start at any initial wave position
        for (; CurrentWave < Waves.Count; CurrentWave++)
        {
            var wave = Waves[CurrentWave];

            if (currentWaveEnum != null) StopCoroutine(currentWaveEnum); // at this point, the last wave should only be waiting to be defeated, if it's still running at all
            StartCoroutine(currentWaveEnum = waveRoutine(wave));

            float timer = 0;
            while (timer < wave.MaxTimeUntilNextWave && !currentWaveDefeated)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        yield return new WaitUntil(() => currentWaveDefeated);

        EndScreen.Victory.StartSequence();
    }

    IEnumerator waveRoutine (Wave wave)
    {
        currentWaveDefeated = false;

        var copyList = new List<EnemyPack>(wave.Packs);
        copyList.ShuffleInPlace();

		for (int i = 0; i < copyList.Count; i++)
        {
            if (i == 0)
            {
                yield return new WaitForSeconds(wave.TimeUntilFirstSpawm);
            }
            else
            {
                yield return new WaitForSeconds(RandomExtra.Range(wave.TimeRangeBetweenSpawns));
            }

			spawnPack(copyList[i]);
        }

        yield return new WaitUntil(() => BaseEnemy.TotalEnemies == 0);

        currentWaveDefeated = true;
    }

    void spawnPack (EnemyPack pack)
    {
        Action spawner = null;

        switch (pack.Category)
        {
            case EnemyCategory.Hamster:
                spawner = () => Instantiate(HamsterPrefab);
                break;

            case EnemyCategory.Butterfly:
                spawner = () => Instantiate(ButterflyPrefab);
                break;

            case EnemyCategory.Goose:
                spawner = spawnGoose;
                break;

            default:
                throw new ArgumentException($"unexpected enemy type {pack.Category}");
        }

        for (int i = 0; i < pack.Number; i++)
        {
            spawner();
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
