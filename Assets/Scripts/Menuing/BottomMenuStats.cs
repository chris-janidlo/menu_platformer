using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BottomMenuStats : MonoBehaviour
{
    [Header("Stats")]
    public MagicColor MagicColor;
    public float HealthPanicThreshold, ManaPanicThreshold, PanicFlashTime, CDFlashTime;
    public int CDFlashes;
    public Color BarNormalColor, BarPanicColor, CDNormalColor;

    [Header("References")]
    public TextMeshProUGUI Title;
    public ColorMapApplierUI BorderColorApplier, TitleColorApplier;
    public Image HealthBar, ManaBar, Cooldown1, Cooldown2;

    IEnumerator healthEnum, manaEnum, cooldown1Enum, cooldown2Enum;

    Mage mage => MageSquad.Instance[MagicColor];

    void Awake ()
    {
        Title.text = MagicColor.ToString();
        BorderColorApplier.Color = MagicColor;
        TitleColorApplier.Color = MagicColor;
    }

    void Update ()
    {
        if (mage.Ability1Cooldown > 0 && !Cooldown1.enabled)
        {
            if (cooldown1Enum != null) StopCoroutine(cooldown1Enum);
            StartCoroutine(cooldown1Enum = cooldown(true));
        }

        if (mage.Ability2Cooldown > 0 && !Cooldown2.enabled)
        {
            if (cooldown2Enum != null) StopCoroutine(cooldown2Enum);
            StartCoroutine(cooldown2Enum = cooldown(false));
        }

        HealthBar.fillAmount = mage.Health.CurrentHealth / mage.Health.MaxHealth;
        if (mage.Health.CurrentHealth <= HealthPanicThreshold)
        {
            if (healthEnum == null)
            {
                healthEnum = panic(HealthBar);
                StartCoroutine(healthEnum);
            }
        }
        else if (healthEnum != null)
        {
            StopCoroutine(healthEnum);
            healthEnum = null;
            HealthBar.color = BarNormalColor;
        }

        ManaBar.fillAmount = mage.Mana / Mage.MaxMana;
        if (mage.Mana <= ManaPanicThreshold)
        {
            if (manaEnum == null)
            {
                manaEnum = panic(ManaBar);
                StartCoroutine(manaEnum);
            }
        }
        else if (manaEnum != null)
        {
            StopCoroutine(manaEnum);
            manaEnum = null;
            ManaBar.color = BarNormalColor;
        }
    }

    IEnumerator panic (Image bar)
    {
        while (true)
        {
            bar.color = BarPanicColor;
            float timer = 0;
            while (timer <= PanicFlashTime)
            {
                bar.color = Color.Lerp(BarPanicColor, BarNormalColor, timer / PanicFlashTime);
                timer += Time.deltaTime;
                yield return null;
            }
            bar.color = BarNormalColor;
            yield return null;
        }
    }

    IEnumerator cooldown (bool is1)
    {
        Func<float> getCD = () => is1 ? mage.Ability1Cooldown : mage.Ability2Cooldown;
        Func<float> getMaxCD = () => is1 ? mage.Ability1CooldownTime : mage.Ability2CooldownTime;
        Func<Image> getCircle = () => is1 ? Cooldown1 : Cooldown2;

        getCircle().enabled = true;

        while (getCD() > 0)
        {
            getCircle().fillAmount = 1 - (getCD() / getMaxCD());
            yield return null;
        }

        getCircle().fillAmount = 1;

        float timer;

        for (int i = 0; i < CDFlashes; i++)
        {
            timer = CDFlashTime;
            while (timer > 0)
            {
                getCircle().color = Color.Lerp(CDNormalColor, MagicColorStats.ColorMap[MagicColor], timer / CDFlashTime);
                timer -= Time.deltaTime;
                yield return null;
            }
        }

        getCircle().enabled = false;
    }
}
