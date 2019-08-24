using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class GoalPart : MonoBehaviour
{
    public event UnityAction Collected;

    public float FlyTime, FlyFinishedDistance;

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
        // get rid of power animation, just show ring
        Destroy(ColoredPart.gameObject);

        // since coin graphic never moves during gameplay
        var targetPos = GoalManager.Instance.CoinGraphic.transform.position;

        Vector2 vel = Vector2.zero;

        while (Vector2.Distance(transform.position, targetPos) > FlyFinishedDistance)
        {
            transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref vel, FlyTime);

            yield return null;
        }

        Collected.Invoke();
        
        Destroy(gameObject);
    }
}
