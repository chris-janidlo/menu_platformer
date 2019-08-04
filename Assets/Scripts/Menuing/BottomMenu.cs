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
    public TextMeshProUGUI WaveDisplay;
    public TextMeshProUGUI HealthAmount;
    public TextMeshProUGUI ManaAmount;

    CanvasGroup group;

    void Start ()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0;
    }

    void Update ()
    {
        WaveDisplay.text = "Wave\n" + EnemySpawner.Instance.CurrentWave;
        HealthAmount.text = "x" + MageSquad.Instance.HealthPots;
        ManaAmount.text = "x" + MageSquad.Instance.ManaPots;
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
