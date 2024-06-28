using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is meant to contain the player's informations and save / load theme
static public class CurrentRunInformations
{
    static public int _goldAmount = 0;

    // Deck related (serves as storage for DeckManager to prepare itself for a room)
    static int _deckMaxCapacity = 100;
    static public List<GameObject> _deck = new();

    // ------
    // DECK BS
    // ------
    static public void AddCardsToDeck(List<GameObject> ARGIncomingCards)
    {
        // Ensures the deck isn't too large
        if (ARGIncomingCards.Count > _deckMaxCapacity)
            Debug.LogError("_Deck was passed with a List too large ! ALL CARDS REFUSED");

        // Cards fit in the deck, so we had them all
        else
            foreach (GameObject card in ARGIncomingCards)
            {
                _deck.Add(card);
                //card.transform.parent = GI._deckContainer.transform;
            }
    }




    // TODO: the whole save / load stuff
    // ------
    // SAVE
    // ------

    static public void Save()
    {

    }

    static public void Load()
    {


    }
}
