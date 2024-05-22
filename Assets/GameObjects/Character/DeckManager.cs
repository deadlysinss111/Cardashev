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
        Init();
    }

    void Update()
    {
        
    }

    void Init()
    {
        _hand = new List<Card>();
        _discardPile = new List<Card>();
        //_remainsInDeck = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<ScriptManager>().GetDeck();
    }

    public void Draw()
    {
        if(_discardPile.Count > 0)
        {
            Debug.Log(_discardPile[0]);
        }

        if (_remainsInDeck.Count == 0)
        {
            _remainsInDeck = _discardPile;
            _discardPile = new List<Card>();
        }
        int rdm = Random.Range(0, _remainsInDeck.Count-1);
        
        GameObject clone = SpawnCard(_remainsInDeck[rdm]);
        _hand.Add(clone.GetComponent<Card>());
        Card toDiscard = _remainsInDeck[rdm];
        clone.GetComponent<Card>()._onDiscard = () => { _discardPile.Add(toDiscard); Debug.Log(toDiscard); };
        _remainsInDeck.RemoveAt(rdm);
        DisplayHand();
    }

    void Discard(Card target)
    {
        _hand.Remove(target);
        target._onDiscard();
        Destroy(target.gameObject);
        DisplayHand();
    }

    public GameObject SpawnCard(Card target)
    {
        GameObject obj = Instantiate(target.gameObject);
        obj.transform.SetParent(GameObject.FindGameObjectsWithTag("Canvas")[0].transform, false);
        obj.transform.localScale = new Vector3(10, 1, 10);
        return obj;
    }

    public void Play(Card target)
    {
        if (GameObject.Find("Queue").GetComponent<QueueComponent>().AddToQueue(target) == true)
        {
            Discard(target);
        }
    }

    private void DisplayHand()
    {
        for(byte i =0; i< _hand.Count; i++)
        {
            _hand[i].transform.localPosition = new Vector3(-400 + 150 * i, -200, 0);
        }
    }
}
