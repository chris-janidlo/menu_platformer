using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using crass;

public class GameStartAnimation : MonoBehaviour
{
    public float BurstShakeTime, BurstShakeAmount;

    [Serializable]
    public class SlideIn
    {
        [Serializable]
        public class SlidingObject
        {
            public RectTransform Transform;
            public Vector2 StartLocation, EndLocation;
        }

        public List<SlidingObject> SlidingObjects;
        public float InitialDelay, Time;
    }

    public SlideIn First, Second;
    public float FadeDelay, FadeTime;
    public Image Fader;

    bool playing;

    IEnumerator Start ()
    {
        playing = true;

        foreach (var item in First.SlidingObjects.Concat(Second.SlidingObjects))
        {
            item.Transform.anchoredPosition = item.StartLocation;
        }

        yield return StartCoroutine(slide(First));
        yield return StartCoroutine(slide(Second));

        yield return new WaitForSeconds(FadeDelay);

        Fader.CrossFadeAlpha(0, FadeTime, false);

        yield return new WaitForSeconds(FadeTime);

        endAnimation();
    }

    void Update ()
    {
        if (playing && Input.anyKey)
        {
            StopAllCoroutines();
            endAnimation();
        }
    }

    IEnumerator slide (SlideIn slideIn)
    {
        yield return new WaitForSeconds(slideIn.InitialDelay);

        Vector2[] vels = new Vector2[slideIn.SlidingObjects.Count];

        float timer = 0;
        while (timer < slideIn.Time)
        {
			for (int i = 0; i < slideIn.SlidingObjects.Count; i++)
            {
				SlideIn.SlidingObject obj = slideIn.SlidingObjects[i];

                obj.Transform.anchoredPosition = Vector2.Lerp(obj.StartLocation, obj.EndLocation, timer / slideIn.Time);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        foreach (var obj in slideIn.SlidingObjects)
        {
            obj.Transform.anchoredPosition = obj.EndLocation;
        }

        CameraCache.Main.ShakeScreen2D(BurstShakeTime, BurstShakeAmount);
    }

    void endAnimation ()
    {
        playing = false;

        foreach (var obj in First.SlidingObjects)
        {
            obj.Transform.anchoredPosition = obj.EndLocation;
        }

        foreach (var obj in Second.SlidingObjects)
        {
            obj.Transform.anchoredPosition = obj.EndLocation;
        }

        Fader.canvasRenderer.SetAlpha(0);
    }
}
