using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;
using static UnityEngine.GraphicsBuffer;

// This class does not control logic yet, and only implements in-room functionnalities
public class DeckManager : MonoBehaviour
{
    protected List<Card> _hand;
    protected List<Card> _remainsInDeck;
    protected List<Card> _discardPile;

    void Start()
    {
        _hand = new List<Card>();
        _discardPile = new List<Card>();
        _remainsInDeck = new List<Card>();

        // Loads every card in the deck
        List<string>toLoad = GI._PManFetcher().GetDeck();
        foreach (string name in toLoad)
        {
            GameObject CARD = (GameObject)Resources.Load(name);
            GameObject card = Instantiate(CARD);
            card.layer = LayerMask.NameToLayer("UI");
            card.transform.SetParent(GameObject.Find("Canvas").transform, false);
            card.transform.localScale = new Vector3(10, 1, 10);
            _remainsInDeck.Add(card.GetComponent<Card>());
            card.SetActive(false);
        }
    }

    public void Draw()
    {
        // If the deck is empty, we shuffle the discard pile into it
        if (_remainsInDeck.Count == 0)
        {
            _remainsInDeck = _discardPile;
            _discardPile = new List<Card> { };
        }

        // We draw a random card
        int rdm = Random.Range(0, _remainsInDeck.Count);

        // We need to duplicate the card's game object so that we can display it an destroy it later easily
        Card obj = _remainsInDeck[rdm];
        obj.gameObject.SetActive(true);
        _hand.Add(obj);

        // Since we drew it, we remove the card from the deck
        _remainsInDeck.RemoveAt(rdm);

        DisplayHand();
    }

    void Discard(Card target)
    {
        _hand.Remove(target);
        _discardPile.Add(target);
        target.gameObject.SetActive(false);

        DisplayHand();
    }

    public void Play(Card target)
    {
        if (GI._PlayerFetcher().GetComponent<QueueComponent>().AddToQueue(target) == true)
        {
            Discard(target);
        }
    }

    // Handling positions in order to have a good looking displaying
    private void DisplayHand()
    {
        for(byte i =0; i< _hand.Count; i++)
        {
            _hand[i].transform.localPosition = new Vector3(-400 + 150 * i, -200, 0);
        }
    }
}
