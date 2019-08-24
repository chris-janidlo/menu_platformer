using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using crass;

public class GoalManager : Singleton<GoalManager>
{
    [Header("Stats")]
    public int GoalPartsUntilVictory;
    public TransformBag GoalPartSpawnLocations;
    public AnimationCurve GoalPartSpawnTimeByNumberSpawned;
    public float CoinGraphicFillAmountAnimationTime;
    public ColorBag ColorDistribution;

    [Header("References")]
    public Image CoinMask;
    public GoalPart GoalPartPrefab;

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
        for (int i = 0; i < GoalPartsUntilVictory; i++)
        {
            var newAmnt = 1 - ((float) i / GoalPartsUntilVictory);
            StartCoroutine(newCoinFillAmount(newAmnt));

            yield return new WaitForSeconds(GoalPartSpawnTimeByNumberSpawned.Evaluate(i));

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
