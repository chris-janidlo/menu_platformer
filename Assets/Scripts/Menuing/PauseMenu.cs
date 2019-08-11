using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenu : Singleton<PauseMenu>
{
    bool inGame, paused;
    CanvasGroup group;

    void Awake ()
    {
        SingletonSetInstance(this, true);
        group = GetComponent<CanvasGroup>();
    }

    void Update ()
    {
        if (Input.GetButtonDown("Menu Cancel")) setPauseState(!paused);
    }

    void OnApplicationFocus (bool hasFocus)
    {
        if (!hasFocus) setPauseState(true);
    }

    public void StartGame ()
    {
        inGame = true;
    }

    void setPauseState (bool value)
    {
        if (!inGame || value == paused) return;

        paused = value;

        Time.timeScale = value ? 0 : 1;
        group.alpha = value ? 1 : 0;
    }
}
