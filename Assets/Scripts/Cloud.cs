using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Cloud : MonoBehaviour
{
    public int Offset;
    public float OffsetSwitchTime;

    public SpriteRenderer Visuals;

    IEnumerator Start ()
    {
        while (true)
        {
            Visuals.transform.localPosition = Vector2.right * Offset;
            Offset *= -1;

            yield return new WaitForSeconds(OffsetSwitchTime);
        }
    }
}
