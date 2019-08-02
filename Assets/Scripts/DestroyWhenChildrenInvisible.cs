using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenChildrenInvisible : MonoBehaviour
{
    void Update ()
	{
		if (GetComponentsInChildren<Renderer>().All(r => !r.isVisible)) Destroy(gameObject);
	}
}
