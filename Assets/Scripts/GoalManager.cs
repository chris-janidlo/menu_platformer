using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using crass;

// TODO: new spawning system:
    // stays with the current color based on a chance given by a curve
        // low number of subsequent spawns of the same color: low chance to change color
        // high number of subsequent spawns: high chance to switch
    // tries to choose spawn points close to you while still avoiding repeats (?)
public class GoalManager : Singleton<GoalManager>
{
    [Header("Stats")]
    public int GoalPartsUntilVictory;
    public TransformBag GoalPartSpawnLocations;
    public float GoalPartSpawnTime;
    public float CoinGraphicFillAmountAnimationTime;
    public ColorBag ColorDistribution;

    [Header("References")]
    public Image CoinMask;
    public GoalPart GoalPartPrefab;

    public int GoalPartsCollected { get; private set; }

    void Awake ()
    {
        SingletonSetInstance(this, true);
        CoinMask.fillAmount = 1;
    }

    public void StartGame ()
    {
        StartCoroutine(gameRoutine());
    }

    IEnumerator gameRoutine ()
    {
        for (; GoalPartsCollected < GoalPartsUntilVictory; GoalPartsCollected++)
        {
            var newAmnt = 1 - ((float) GoalPartsCollected / GoalPartsUntilVictory);
            StartCoroutine(newCoinFillAmount(newAmnt));

            yield return new WaitForSeconds(GoalPartSpawnTime);

            bool currentCollected = false;

            var item = Instantiate(GoalPartPrefab, GoalPartSpawnLocations.GetNext().position, Quaternion.identity);
            item.Initialize(ColorDistribution.GetNext());
            item.Collected += () => currentCollected = true;

            yield return new WaitUntil(() => currentCollected);
        }

        StartCoroutine(newCoinFillAmount(1));

        EndScreen.Victory.StartSequence();
    }

    IEnumerator newCoinFillAmount (float fillAmount)
    {
        float velocity = 0;

        while (!Mathf.Approximately(CoinMask.fillAmount, fillAmount))
        {
            CoinMask.fillAmount = Mathf.SmoothDamp(CoinMask.fillAmount, fillAmount, ref velocity, CoinGraphicFillAmountAnimationTime);

            yield return null;
        }

        CoinMask.fillAmount = fillAmount;
    }
}
