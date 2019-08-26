using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class ColorMapApplierUI : MonoBehaviour
{
    public MagicColor Color;

    void Start ()
    {
        ChangeColor(Color);

        MagicColorStats.ColorMapChanged += () => ChangeColor(Color);
    }

    public void ChangeColor (MagicColor color)
    {
        Color = color;

        GetComponent<Graphic>().color = MagicColorStats.ColorMap[Color];
    }
}
