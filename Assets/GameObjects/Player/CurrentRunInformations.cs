using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

// This class is meant to contain the player's informations and save / load theme
static public class CurrentRunInformations
{
    static public int _goldAmount = 100;
    static public int _playerHP;

    // Deck related (serves as storage for DeckManager to prepare itself for a room)
    static int _deckMaxCapacity = 100;
    static public List<GameObject> _deck = new();

    // ------
    // DECK BS
    // ------
    static public void AddCardsToDeck(List<string> ARGIncomingCards)
    {
        // Ensures the deck isn't too large
        if (ARGIncomingCards.Count > _deckMaxCapacity)
            Debug.LogError("_Deck was passed with a List too large ! ALL CARDS REFUSED");

        // Cards fit in the deck, so we add them all
        else
            foreach (string name in ARGIncomingCards)
            {
                GameObject card = Card.Instantiate(name);
                _deck.Add(card);
                card.transform.SetParent(GI._deckContainer.transform);
            }
    }
    
    static public void AddCardsToDeck(List<GameObject> ARGIncomingCards)
    {
        // Ensures the deck isn't too large
        if (ARGIncomingCards.Count > _deckMaxCapacity)
            Debug.LogError("_Deck was passed with a List too large ! ALL CARDS REFUSED");

        // Cards fit in the deck, so we had them all
        else
            foreach (GameObject card in ARGIncomingCards)
            {
                Debug.Log("the card is : "+card.name);
                _deck.Add(card);
                card.transform.SetParent(GI._deckContainer.transform);
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
