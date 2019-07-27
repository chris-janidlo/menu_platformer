using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class BottomMenu : MonoBehaviour
{
    [Header("Stats")]
    public float FadeInTime;

    [Header("References")]
    public TextMeshProUGUI ManaAmount;
    public TextMeshProUGUI HealthAmount;

    CanvasGroup group;

    void Start ()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0;
    }

    void Update ()
    {
        ManaAmount.text = "x" + MageSquad.Instance.ManaPots;
        HealthAmount.text = "x" + MageSquad.Instance.HealthPots;
    }

    public void StartGame ()
    {
        StartCoroutine(fadeIn());
    }

    IEnumerator fadeIn ()
    {
        float timer = 0;

        while (timer <= FadeInTime)
        {
            group.alpha = timer / FadeInTime;
            timer += Time.deltaTime;
            yield return null;
        }

        group.alpha = 1;
    }
}
