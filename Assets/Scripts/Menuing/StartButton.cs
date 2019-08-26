using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public BottomMenu BottomMenu;
    public FakeCopyrightFadeOut FakeCopyrightFadeOut;
    public Button StartButt, OptionsButton;
    public OptionsMenu OptionsMenu;

    void Start ()
    {
        StartButt.onClick.AddListener(() => StartCoroutine(startRoutine()));
        OptionsButton.onClick.AddListener(() => clickOptions());
    }

    IEnumerator startRoutine ()
    {
        MageSquad.Instance.StartGame();
        PauseMenu.Instance.StartGame();
        GoalManager.Instance.StartGame();
        EnemySpawner.Instance.StartGame();
        PlayMenuManager.Instance.StartGame();
        Destroy(StartMenuSelectionFollower.Instance.gameObject);

        StartButt.interactable = false;
        OptionsButton.interactable = false;

        FakeCopyrightFadeOut.Fade();
        yield return new WaitForSeconds(FakeCopyrightFadeOut.FadeOutTime);
        BottomMenu.StartGame();
    }

    void clickOptions ()
    {
        StartButt.interactable = false;
        OptionsButton.interactable = false;

        OptionsMenu.SetActive(true);
    }
}
