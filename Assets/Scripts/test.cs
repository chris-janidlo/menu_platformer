using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Hamster HamsterPrefab;

    void Start ()
    {
        for (int i = 0; i < 10; i++)
        {
            Instantiate(HamsterPrefab).Initialize((MagicColor) Random.Range(0, 2));
        }
    }
}
