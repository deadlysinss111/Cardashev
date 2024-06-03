using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;
using static UnityEngine.GraphicsBuffer;

public class EnemyDeckManager : DeckManager
{
    Card _activeCard;
    float _activeCardEndTime;

    void Start()
    {
        Init();
    }

    void Init()
    {
        _hand = new List<Card>();
        _discardPile = new List<Card>();
        List<string> toLoad = GetComponent<BasicEnemyHandler>().GetDeck();
        _remainsInDeck = new List<Card>();
        foreach (string name in toLoad)
        {
            GameObject go = new GameObject(name);
            Type t = Type.GetType(name);
            go.transform.parent = gameObject.transform;
            go.AddComponent(t);
            _remainsInDeck.Add((Card)go.GetComponent(t));

        }
    }

    private void Update()
    {
        if (_activeCard is null) return;
        
        if (Time.time > _activeCardEndTime)
        {
            _activeCard = null;
        }
    }

    // called when the player draws a card
    public new void Draw()
    {
        if (_activeCard is not null) return;

        // if the deck is empty, we shuffle the discard pile into it
        if (_remainsInDeck.Count == 0)
        {
            _remainsInDeck = _discardPile;
            _discardPile = new List<Card>();
        }

        // we draw a random card
        int rdm = UnityEngine.Random.Range(0, _remainsInDeck.Count - 1);

        // we need to duplicate the card's game object so that we can display it an destroy it later easily
        Card obj = _remainsInDeck[rdm];
        obj.gameObject.SetActive(true);
        _hand.Add(obj);

        // since we drew it, we remove the card from the deck
        _remainsInDeck.RemoveAt(rdm);
        _discardPile.Add(obj);

        //DisplayHand();
        Play(obj);
    }

    void Discard(Card target)
    {
        _hand.Remove(target);

        //Destroy(target.gameObject);

        //DisplayHand();
    }

    // this function makes a card apperaing on screen
    public new GameObject SpawnCard(Card target)
    {
        GameObject obj = Instantiate(target.gameObject);
        //obj.transform.SetParent(GameObject.FindGameObjectsWithTag("Canvas")[0].transform, false);
        //obj.transform.localScale = new Vector3(10, 1, 10);
        return obj;
    }

    public void Play(Card target)
    {
        //if (GameObject.Find("Player").GetComponent<QueueComponent>().AddToQueue(target) == true)
        //{
        target.Effect(gameObject);
        _activeCard = target;
        _activeCardEndTime = Time.time + target._duration;
        Discard(target);
        //}
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
