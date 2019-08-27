using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public TextMeshProUGUI StartingLevel;
    public Button StartingLevelUp, StartingLevelDown, Back, Parent, Uncle;
    public Toggle Colorblind;
    public GameObject TopLevel;

    bool active;

    void Start ()
    {
        StartingLevelUp.interactable = false;
        StartingLevelDown.interactable = false;
        Back.interactable = false;
        Colorblind.interactable = false;

        Back.onClick.AddListener(() => SetActive(false));

        Colorblind.isOn = MagicColorStats.ColorBlindMode;
        Colorblind.onValueChanged.AddListener(b => MagicColorStats.ColorBlindMode = b);

        StartingLevelUp.onClick.AddListener(() => GoalManager.Instance.StartingLevel++);
        StartingLevelDown.onClick.AddListener(() => GoalManager.Instance.StartingLevel--);
    }

    void Update ()
    {
        if (!active) return;

        StartingLevel.text = GoalManager.Instance.StartingLevel.ToString();

        StartingLevelUp.interactable = GoalManager.Instance.StartingLevel < GoalManager.Instance.GoalPartsUntilVictory - 1;
        StartingLevelDown.interactable = GoalManager.Instance.StartingLevel > 0;

        if (Input.GetButtonDown("Menu Cancel"))
        {
            SetActive(false);
        }
    }

    public void SetActive (bool value)
    {
        if (active == value) return;
        active = value;

        StartingLevelUp.interactable = value;
        StartingLevelDown.interactable = value;
        Back.interactable = value;
        Colorblind.interactable = value;

        if (!value)
        {
            Parent.interactable = true;
            Uncle.interactable = true;
        }

        EventSystemCache.Instance.SetSelected((value ? StartingLevelUp : Parent).gameObject);

        TopLevel.SetActive(value);
    }
}
