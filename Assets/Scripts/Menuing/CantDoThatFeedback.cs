using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using crass;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(Rigidbody2D))]
public class CantDoThatFeedback : Singleton<CantDoThatFeedback>
{
    public Vector2 InitialSpeedRange, ArcAngleRange;
    public float MessageTime;

    TextMeshProUGUI text;
    Rigidbody2D rb;

    void Awake ()
    {
        SingletonSetInstance(this, true);

        text = GetComponent<TextMeshProUGUI>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void DisplayMessage (string message)
    {
        StopAllCoroutines();
        StartCoroutine(messageRoutine(message));
    }

    IEnumerator messageRoutine (string message)
    {
        text.text = message;
        transform.localPosition = Vector2.zero;

        var direction = Quaternion.AngleAxis(RandomExtra.Range(ArcAngleRange), Vector3.forward) * Vector3.down;

        rb.velocity = RandomExtra.Range(InitialSpeedRange) * direction;

        yield return new WaitForSeconds(MessageTime);

        rb.velocity = Vector2.zero;
        text.text = "";
    }
}
