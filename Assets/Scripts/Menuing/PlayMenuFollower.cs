using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

public class PlayMenuFollower : MonoBehaviour
{
    public float MinX, MaxX, MaxY;

    public float SwitchSmoothTimeInitial, SwitchSmoothTimeDecay, SwitchDestinationDistance;

    Transform target;
    Vector3 switchTarget;
    bool switching;

    void Start ()
    {
        MageSquad.Instance.ActiveMageChanged.AddListener(mageChange);
        target = MageSquad.Instance.ActiveMage.transform;
    }

    void Update ()
    {
        var targetPos = switching ? switchTarget : target.position;

        transform.position = new Vector2
        (
            Mathf.Clamp(targetPos.x, MinX, MaxX),
            Mathf.Min(targetPos.y, MaxY)
        );
    }

    void mageChange (Mage oldMage, Mage newMage)
    {
        StopAllCoroutines();
        StartCoroutine(mageChangeRoutine(oldMage, newMage));
    }

    IEnumerator mageChangeRoutine (Mage oldMage, Mage newMage)
    {
        switching = true;
        var vel = Vector3.zero;

        var smoothTime = SwitchSmoothTimeInitial;

        while (Vector3.Distance(transform.position, newMage.transform.position) > SwitchDestinationDistance)
        {
            switchTarget = Vector3.SmoothDamp(transform.position, newMage.transform.position, ref vel, smoothTime);

            smoothTime -= SwitchSmoothTimeDecay * Time.deltaTime;

            yield return null;
        }

        target = newMage.transform;
        switching = false;
    }
}
