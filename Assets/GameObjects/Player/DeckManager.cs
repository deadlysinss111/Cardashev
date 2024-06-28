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
    protected List<GameObject> _hand;
    protected List<GameObject> _remainsInDeck;
    protected List<GameObject> _discardPile;

    protected float _drawCooldown;
    protected int _curHandSize;

    void Start()
    {
        _hand = new List<GameObject>();
        _discardPile = new List<GameObject>();
        _remainsInDeck = new List<GameObject>();
        _drawCooldown = 0;
        _curHandSize = 0;

        LoadDeck();
    }

    private void Update()
    {
        _drawCooldown -= Time.deltaTime;
        if (_drawCooldown  <= 0)
        {
            if (_hand.Count < 5)
            {
                Draw();
                _drawCooldown = 3;
            }
        }
    }

    public void Draw()
    {
        // If the deck is empty, we shuffle the discard pile into it
        if (_remainsInDeck.Count == 0)
        {
            if (_discardPile.Count == 0)
                return;
            _remainsInDeck = _discardPile;
            _discardPile = new List<GameObject> { };
        }

        // We draw a random card
        int rdm = Random.Range(0, _remainsInDeck.Count);

        // We need to duplicate the card's game object so that we can display it an destroy it later easily
        GameObject obj = _remainsInDeck[rdm];
        obj.gameObject.SetActive(true);
        _hand.Add(obj);

        // Since we drew it, we remove the card from the deck
        _remainsInDeck.RemoveAt(rdm);

        DisplayHand();
    }

    void Discard(GameObject target)
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
            Discard(target.gameObject);
        }
    }

    // Handling positions in order to have a good looking displaying
    private void DisplayHand()
    {
        for(byte i =0; i< _hand.Count; i++)
        {
            _hand[i].transform.localPosition = new Vector3(-400 + 150 * i, -300, 0);
        }
    }

    public void LoadDeck()
    {
        List<GameObject> toLoad = GI._PManFetcher().GetDeck();
        foreach (GameObject card in toLoad)
        {
            _remainsInDeck.Add(card);
            card.transform.parent = GameObject.Find("Canvas").transform;
        }
    }
    public void UnloadDeck()
    {
        foreach (GameObject card in _remainsInDeck)
        {
            card.transform.parent = GI._deckContainer.transform;
        }
        foreach (GameObject card in _hand)
        {
            card.transform.parent = GI._deckContainer.transform;
        }
        foreach (GameObject card in _discardPile)
        {
            card.transform.parent = GI._deckContainer.transform;
        }
    }
}
