using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorMap
{
    public static Dictionary<MagicColor, Color> Value = new Dictionary<MagicColor, Color>
    {
        { MagicColor.Red, Color.red },
        { MagicColor.Green, Color.green },
        { MagicColor.Blue, Color.blue }
    };
}

public enum MagicColor
{
    Red, Green, Blue
}
