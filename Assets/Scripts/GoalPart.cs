using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class GoalPart : MonoBehaviour
{
    public event UnityAction Collected;

    public float FlyTime, FlyFinishedDistance, ShrinkTime;

    public ColorMapApplier ColoredPart;

    MagicColor color;
    bool flying;

    public void Initialize (MagicColor color)
    {
        this.color = color;
        ColoredPart.ChangeColor(color);
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

        Collected.Invoke();


        // shrink
        // TODO: why is the goal part behind the goal coin UI element when it wasn't before

        vel = Vector2.zero;

        while (!Mathf.Approximately(transform.localScale.x, 0))
        {
            transform.localScale = Vector2.SmoothDamp(transform.localScale, Vector2.zero, ref vel, ShrinkTime);

            yield return null;
        }

        Destroy(gameObject);
    }
}
