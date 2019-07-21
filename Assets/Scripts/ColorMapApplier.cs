using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ColorMapApplier : MonoBehaviour
{
    public MagicColor Color;

    void Start ()
    {
        GetComponent<SpriteRenderer>().color = ColorMap.Value[Color];
    }
}
