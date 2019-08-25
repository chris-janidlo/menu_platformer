using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [Header("Stats")]
    public EnemyBag Enemies;
    public AnimationCurve SpawnTimeByGoalsCollected;

    [Range(0, 1)]
    public float ItemDropRate;
    public ItemBag ItemDropDistribution;

    public float StartGameReadyTime;

    public TransformBag ButterflySpawnLocations, HamsterSpawnLocations;

    [Range(0, 1)]
    public float GooseTargetsActiveChance;

    [Header("References")]
    public Butterfly ButterflyPrefab;
    public Hamster HamsterPrefab;
    public Goose GoosePrefab;

    List<Mage> currentGooseTargets = new List<Mage>();

    bool apocalypsed;

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }

    void Update ()
    {
        if (!apocalypsed && EndScreen.GameIsComplete)
        {
            endGame();            
        }
    }

    public void StartGame ()
    {
        StartCoroutine(gameRoutine());
    }

    void endGame ()
    {
        apocalypsed = true;

        StopAllCoroutines();

        List<GameObject> toDestroy = new List<GameObject>();

        toDestroy.AddRange(FindObjectsOfType<BaseEnemy>().Select(e => e.gameObject));
        toDestroy.AddRange(FindObjectsOfType<HamsterFart>().Select(e => e.gameObject));
        toDestroy.AddRange(FindObjectsOfType<GooseLaser>().Select(e => e.gameObject));

        foreach (var obj in toDestroy)
        {
            Destroy(obj);
        }
    }

    IEnumerator gameRoutine ()
    {
        yield return new WaitForSeconds(StartGameReadyTime);

        while (true)
        {
            spawnEnemy(Enemies.GetNext());

            yield return new WaitForSeconds(SpawnTimeByGoalsCollected.Evaluate(GoalManager.Instance.GoalPartsCollected));
        }
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

        var nonActiveValidTargets = new List<Mage>
        {
            MageSquad.Instance.RedMage,
            MageSquad.Instance.GreenMage,
            MageSquad.Instance.BlueMage
        }
        .Where(m => !m.Active && !currentGooseTargets.Contains(m))
        .ToList();

        if ((nonActiveValidTargets.Count == 0 || RandomExtra.Chance(GooseTargetsActiveChance)) && !currentGooseTargets.Contains(MageSquad.Instance.ActiveMage))
        {
            target = MageSquad.Instance.ActiveMage;
        }
        else if (nonActiveValidTargets.Count > 0)
        {
            target = nonActiveValidTargets.PickRandom();
        }
        else
        {
            return;
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
