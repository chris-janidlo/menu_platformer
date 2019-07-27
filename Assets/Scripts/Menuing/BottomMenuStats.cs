using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BottomMenuStats : MonoBehaviour
{
    [Header("Stats")]
    public MagicColor MagicColor;
    public float HealthPanicThreshold, ManaPanicThreshold, PanicFadeTime;
    public Color NormalColor, PanicColor;

    [Header("References")]
    public TextMeshProUGUI Title;
    public ColorMapApplierUI BorderColorApplier, TitleColorApplier;
    public Image HealthBar, ManaBar;

    IEnumerator healthEnum, manaEnum;

    void Awake ()
    {
        Title.text = MagicColor.ToString();
        BorderColorApplier.Color = MagicColor;
        TitleColorApplier.Color = MagicColor;
    }

    void Update ()
    {
        var mage = MageSquad.Instance[MagicColor];

        HealthBar.fillAmount = mage.Health / Mage.MaxHealth;
        if (mage.Health <= HealthPanicThreshold)
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
            HealthBar.color = NormalColor;
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
            ManaBar.color = NormalColor;
        }
    }

    IEnumerator panic (Image bar)
    {
        while (true)
        {
            bar.color = PanicColor;
            float timer = 0;
            while (timer <= PanicFadeTime)
            {
                bar.color = Color.Lerp(PanicColor, NormalColor, timer / PanicFadeTime);
                timer += Time.deltaTime;
                yield return null;
            }
            bar.color = NormalColor;
            yield return null;
        }
    }
}
