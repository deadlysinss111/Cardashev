using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;
using static UnityEngine.GraphicsBuffer;

public class DeckManager : MonoBehaviour
{
    private List<Card> _hand;
    private List<Card> _remainsInDeck;
    private List<Card> _discardPile;

    void Start()
    {
        _hand = new List<Card>();
        _discardPile = new List<Card>();
        List<string>toLoad = GameObject.Find("Player").GetComponent<PlayerManager>().GetDeck();
        _remainsInDeck = new List<Card>();
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

    // called when the player draws a card
    public void Draw()
    {
        // if the deck is empty, we shuffle the discard pile into it
        if (_remainsInDeck.Count == 0)
        {
            _remainsInDeck = _discardPile;
            _discardPile = new List<Card> { };
        }

        // we draw a random card
        int rdm = Random.Range(0, _remainsInDeck.Count-1);

        // we need to duplicate the card's game object so that we can display it an destroy it later easily
        Card obj = _remainsInDeck[rdm];
        obj.gameObject.SetActive(true);
        _hand.Add(obj);

        // since we drew it, we remove the card from the deck
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
        if (GameObject.Find("Player").GetComponent<QueueComponent>().AddToQueue(target) == true)
        {
            Discard(target);
        }
    }

    // handling positions in order to have a good looking displaying
    private void DisplayHand()
    {
        for(byte i =0; i< _hand.Count; i++)
        {
            _hand[i].transform.localPosition = new Vector3(-400 + 150 * i, -200, 0);
        }
    }
}
