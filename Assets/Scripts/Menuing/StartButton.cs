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
        GetComponent<Button>().onClick.AddListener(() => {
            MageSquad.Instance.StartGame();
            PlayMenuManager.Instance.StartGame();
            BottomMenu.StartGame();
            FakeCopyrightFadeOut.StartGame();
            Destroy(StartMenuSelectionFollower.Instance.gameObject);

            foreach (var sibling in Siblings)
            {
                sibling.interactable = false;
            }
        });
    }
}
