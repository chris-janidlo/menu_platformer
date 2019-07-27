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
        GetComponent<Graphic>().color = ColorMap.Value[Color];
    }
}
