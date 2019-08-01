using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ColorMapApplier : MonoBehaviour
{
    public MagicColor Color;

    SpriteRenderer _sr;
    SpriteRenderer sr
    {
        get
        {
            if (_sr == null) _sr = GetComponent<SpriteRenderer>();
            return _sr;
        }
    }

    void Start ()
    {
        if (sr.color == UnityEngine.Color.white) ChangeColor(Color);
    }

    public void ChangeColor (MagicColor color)
    {
        Color = color;
        sr.color = MagicColorStats.ColorMap[Color];
    }
}
