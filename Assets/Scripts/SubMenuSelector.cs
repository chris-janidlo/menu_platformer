using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SubMenuSelector : MonoBehaviour
{
    public Button MainButton, FirstSelectedChild;
    // siblings can include self or not, either way works
    public List<Button> Siblings, Children;
    public RectTransform ChildContainer;

    bool subMenuActive;

    void Start ()
    {
        MainButton.onClick.AddListener(() => setSubMenuState(true));
    }

	public void Update ()
	{
        if (Input.GetButtonDown("Menu Cancel") && subMenuActive)
        {
            setSubMenuState(false);
        }
	}

    void setSubMenuState (bool value)
    {
        MainButton.interactable = !value;

        foreach (var sibling in Siblings)
        {
            sibling.interactable = !value;
        }

        foreach (var child in Children)
        {
            child.interactable = value;
        }

        ChildContainer.gameObject.SetActive(value);

        EventSystemCache.Main.SetSelectedGameObject(value ? FirstSelectedChild.gameObject : MainButton.gameObject);

        subMenuActive = value;
    }
}
