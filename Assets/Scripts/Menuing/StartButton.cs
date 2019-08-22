using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartButton : MonoBehaviour
{
    public BottomMenu BottomMenu;
    public FakeCopyrightFadeOut FakeCopyrightFadeOut;
    public List<Button> Siblings;

    void Start ()
    {
        GetComponent<Button>().onClick.AddListener(() => StartCoroutine(startRoutine()));
    }

    IEnumerator startRoutine ()
    {
        MageSquad.Instance.StartGame();
        PauseMenu.Instance.StartGame();
        GoalManager.Instance.StartGame();
        EnemySpawner.Instance.StartGame();
        PlayMenuManager.Instance.StartGame();
        Destroy(StartMenuSelectionFollower.Instance.gameObject);

        foreach (var sibling in Siblings)
        {
            sibling.interactable = false;
        }

        FakeCopyrightFadeOut.Fade();
        yield return new WaitForSeconds(FakeCopyrightFadeOut.FadeOutTime);
        BottomMenu.StartGame();
    }
}
