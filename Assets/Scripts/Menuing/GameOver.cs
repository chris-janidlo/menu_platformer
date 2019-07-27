using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using crass;

[RequireComponent(typeof(CanvasGroup))]
public class GameOver : Singleton<GameOver>
{
    public float FadeTime, PromptDelay, PromptFadeTime;
    public TextMeshProUGUI Prompt;

    CanvasGroup group;

    void Awake ()
    {
        SingletonSetInstance(this, true);
        group = GetComponent<CanvasGroup>();
    }

    [ContextMenu("test run")]
    public void StartSequence ()
    {
        StartCoroutine(gameOverRoutine());
    }

    IEnumerator gameOverRoutine ()
    {
        Prompt.alpha = 0;
        PlayMenuManager.Instance.gameObject.SetActive(false);

        float timer = 0;
        while (timer < FadeTime)
        {
            group.alpha = timer / FadeTime;
            timer += Time.deltaTime;
            yield return null;
        }
        group.alpha = 1;

        yield return new WaitForSecondsRealtime(PromptDelay);

        timer = 0;
        while (timer < PromptFadeTime)
        {
            Prompt.alpha = timer / PromptFadeTime;
            timer += Time.deltaTime;
            yield return null;
        }
        Prompt.alpha = 1;

        while (true)
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene("Main");
            }
            yield return null;
        }
    }
}
