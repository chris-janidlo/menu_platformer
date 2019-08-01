using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteRendererExtensions
{
    public static void SetAlpha (this SpriteRenderer sr, float alpha)
    {
        var col = sr.color;
        col.a = alpha;
        sr.color = col;
    }
}
