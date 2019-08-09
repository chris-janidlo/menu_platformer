using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenChildrenInvisible : MonoBehaviour
{
	public bool ShouldDestroy;

	public bool ChildrenInvisible => GetComponentsInChildren<Renderer>().All(r => !r.isVisible);

    void Update ()
	{
		if (ShouldDestroy && ChildrenInvisible) Destroy(gameObject);
	}
}
