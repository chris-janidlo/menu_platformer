using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using crass;

// TODO: try to choose spawn points close to you while still avoiding repeats (?)
public class GoalManager : Singleton<GoalManager>
{
    [Header("Stats")]
    public int GoalPartsUntilVictory;
    public TransformBag GoalPartSpawnLocations;
    public float GoalPartSpawnTime;
    public float CoinGraphicFillAmountAnimationTime;
    public AnimationCurve ChanceToSwitchColorsByColorStreak;

    [Header("References")]
    public Image CoinMask;
    public GoalPart GoalPartPrefab;

    public int GoalPartsCollected { get; private set; }

    MagicColor currentColor;
    int colorStreak;
    ColorBag colors;

    void Awake ()
    {
        SingletonSetInstance(this, true);
        CoinMask.fillAmount = 1;

        colors = new ColorBag();
        colors.Items = new List<MagicColor> { MagicColor.Red, MagicColor.Green, MagicColor.Blue };
        colors.AvoidRepeats = true;

        currentColor = colors.GetNext();
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
            item.Initialize(getColor());
            item.Collected.AddListener(() => currentCollected = true);

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

    MagicColor getColor ()
    {
        colorStreak++;

        var chance = ChanceToSwitchColorsByColorStreak.Evaluate(colorStreak);
        if (RandomExtra.Chance(chance))
        {
            currentColor = colors.GetNext();
            colorStreak = 0;
        }

        return currentColor;
    }
}
