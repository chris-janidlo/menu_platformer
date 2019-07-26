using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class BottomMenu : MonoBehaviour
{
    public float FadeInTime;

    CanvasGroup group;

    void Start ()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0;
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
