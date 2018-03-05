using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card_Model : MonoBehaviour {

    Image sourceImage;
    public Sprite[] faces;
    public Sprite cardBack;

    public int cardIndex;

    public void ToggleFace(bool showFace)
    {

        if(showFace)
        {
            sourceImage.sprite = faces[cardIndex];
        }
        else
        {
            sourceImage.sprite = cardBack;
        }
    }

    void Awake()
    {
        sourceImage = GetComponent<Image>();
    }

}
