using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCard : MonoBehaviour {

    CardFlipper flipper;
    Card_Model cardModel;
    int cardIndex = 0;

    public static GameObject[] cards;

    public void Flip()
    {
        cardIndex = 0;
        foreach (GameObject card in cards)
        {
            cardModel = card.GetComponent<Card_Model>();
            flipper = card.GetComponent<CardFlipper>();
            flipper.FlipCard(cardModel.cardBack, cardModel.faces[cardIndex], cardIndex);
            cardIndex++;
        }
    }
}
