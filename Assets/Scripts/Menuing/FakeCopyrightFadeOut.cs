using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FakeCopyrightFadeOut : MonoBehaviour
{
    public float FadeOutTime;

    public void Fade ()
    {
        StartCoroutine(fadeOut());
    }

    IEnumerator fadeOut ()
    {
        var text = GetComponent<TextMeshProUGUI>();
        float timer = 0;
        while (timer <= FadeOutTime)
        {
            text.alpha = 1 - (timer / FadeOutTime);
            timer += Time.deltaTime;
            yield return null;
        }
        text.alpha = 0;
    }
}
