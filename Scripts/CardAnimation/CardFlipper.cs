using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour {

    Image sourceImage;
    Card_Model model;

    public AnimationCurve scaleCurve;
    public float duration = 0.5f;

    void Awake()
    {
        sourceImage = GetComponent<Image>();
        model = GetComponent<Card_Model>();
    }

    public void FlipCard (Sprite startImage, Sprite endImage, int cardIndex)
    {
        StopCoroutine(Flip(startImage, endImage, cardIndex));
        StartCoroutine(Flip(startImage, endImage, cardIndex));
    }

    IEnumerator Flip(Sprite startImage, Sprite endImage, int cardIndex)
    {
        sourceImage.sprite = startImage;

        float time = 0f;
        while (time <= 1f)
        {
            float scale = scaleCurve.Evaluate(time);
            time = time + Time.deltaTime / duration;

            Vector3 localScale = transform.localScale;
            localScale.x = scale;
            transform.localScale = localScale;

            if (time >= 0.5f)
            {
                sourceImage.sprite = endImage;
            }

            yield return new WaitForFixedUpdate();

        }

        if(cardIndex == -1)
        {
            model.ToggleFace(false);
        }
        else
        {
            model.cardIndex = cardIndex;
            model.ToggleFace(true);
        }
    }
}
