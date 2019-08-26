using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class GoalPart : MonoBehaviour
{
    public UnityEvent Collected;

    public float GrowTime, ShrinkTime, ShrinkFinishedSize, FlyTime, FlyFinishedDistance;

    public ColorMapApplier ColoredPart;

    MagicColor color;
    bool flying;

    void Start ()
    {
        transform.localScale = Vector3.zero;
    }

    public void Initialize (MagicColor color)
    {
        this.color = color;
        ColoredPart.ChangeColor(color);

        StartCoroutine(growRoutine());
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (flying) return;

        var mage = other.GetComponent<Mage>();

        if (!mage.Active) return;

        bool impossibleForColorsToMatch = MageSquad.Instance.GreenMage.Health.Dead && MageSquad.Instance[color].Health.Dead;

        if (impossibleForColorsToMatch || mage.Color == color)
        {
            flying = true;
            StartCoroutine(collectRoutine());
        }
        else
        {
            CantDoThatFeedback.Instance.DisplayMessage($"only {color.ToString()} Mage can pick this up!");
        }
    }

    IEnumerator growRoutine ()
    {
        var vel = Vector3.zero;

        transform.localScale = Vector3.zero;

        while (!Mathf.Approximately(transform.localScale.x, 1))
        {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.one, ref vel, GrowTime);

            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    IEnumerator collectRoutine ()
    {
        // fly

        // since coin graphic never moves during gameplay
        var targetPos = GoalManager.Instance.CoinMask.transform.position;

        Vector2 vel = Vector2.zero;

        while (Vector2.Distance(transform.position, targetPos) > FlyFinishedDistance)
        {
            transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref vel, FlyTime);

            yield return null;
        }

        // shrink

        vel = Vector2.zero;

        while (transform.localScale.x > ShrinkFinishedSize)
        {
            transform.localScale = Vector2.SmoothDamp(transform.localScale, Vector2.zero, ref vel, ShrinkTime);

            yield return null;
        }

        Collected.Invoke();

        Destroy(gameObject);
    }
}
