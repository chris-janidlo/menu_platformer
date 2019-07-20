using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ColorMapApplier : MonoBehaviour
{
    public MagicColor Color;

    SpriteRenderer render;

    void Start ()
    {
        render.color = ColorMap.Value[Color];
    }
}
