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
    public List<Card> _remainsInDeck;
    private List<Card> _discardPile;

    void Start()
    {
        _hand = new List<Card>();
        _discardPile = new List<Card>();
        //_remainsInDeck = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<ScriptManager>().GetDeck();
    }

    // called when the player draws a card
    public void Draw()
    {
        // if the deck is empty, we shuffle the discard pile into it
        if (_remainsInDeck.Count == 0)
        {
            _remainsInDeck = _discardPile;
            //_discardPile = new List<Card>();
            _discardPile.Clear();
        }

        // we draw a random card
        int rdm = Random.Range(0, _remainsInDeck.Count);

        // we need to duplicate the card's game object so that we can display it an destroy it later easily
        GameObject clone = SpawnCard(_remainsInDeck[rdm]);
        _hand.Add(clone.GetComponent<Card>());
        Card toDiscard = _remainsInDeck[rdm];

        // it's kinda dirty but we use the closure to easily keep the card in memory and add it to discard pile later
        clone.GetComponent<Card>()._trigger += () => { _discardPile.Add(toDiscard); Debug.Log(toDiscard); };

        // since we drew it, we remove the card from the deck
        _remainsInDeck.RemoveAt(rdm);

        DisplayHand();
    }

    void Discard(Card target)
    {
        _hand.Remove(target);

        Destroy(target.gameObject);

        DisplayHand();
    }

    // this function makes a card apperaing on screen
    public GameObject SpawnCard(Card target)
    {
        GameObject obj = Instantiate(target.gameObject);
        obj.transform.SetParent(GameObject.FindGameObjectsWithTag("Canvas")[0].transform, false);
        obj.transform.localScale = new Vector3(10, 1, 10);
        return obj;
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
