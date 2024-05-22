using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;
using static UnityEngine.GraphicsBuffer;

//public struct ActiveCard
//{
//    public ActiveCard(Card reference, GameObject obj)
//    {
//        _reference = reference;
//        _obj = obj;
//    }
//    Card _reference;
//    GameObject _obj;
//}

public class DeckManager : MonoBehaviour
{
    private Dictionary<int, GameObject> _hand;
    public List<Card> _tempCauseUnityShit;
    private Dictionary<int, Card> _deck;
    private List<int> _remainsInDeck;
    private List<int> _discardPile;
    private int _range;

    void Start()
    {
        Init();

        // everything below is temporary shit
        _deck = new Dictionary<int, Card>();
        _remainsInDeck = new List<int>();
        for(int i=0; i<_tempCauseUnityShit.Count; i++)
        {
            _tempCauseUnityShit[i]._id = i;
            _deck.Add(i, _tempCauseUnityShit[i]);
            _remainsInDeck.Add(i);
        }
        _range = _remainsInDeck.Count -1;
    }

    void Update()
    {
        
    }

    void Init()
    {
        _hand = new Dictionary<int, GameObject>();
        _discardPile = new List<int>();
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
            _discardPile = new List<int>();
        }
        int rdm = 0;
        //while(_remainsInDeck.Contains(rdm) == false)
        //{
            rdm = Random.Range(0, _range);
        //    Debug.Log(_remainsInDeck);
        //}
        
        GameObject clone = SpawnCard(rdm);
        Debug.Log("we hit there 1");
        _hand.Add(rdm, clone);
        Debug.Log("we hit there 2");
        _remainsInDeck.Remove(rdm);
        Debug.Log("we hit there 3");
        DisplayHand();
    }

    void Discard(int id)
    {
        GameObject found = _hand[id];
        _discardPile.Add(id);
        Destroy(found);
        _hand.Remove(id);
        DisplayHand();
    }

    public GameObject SpawnCard(int id)
    {
        GameObject obj = Instantiate(_deck[id].gameObject);
        obj.transform.SetParent(GameObject.FindGameObjectsWithTag("Canvas")[0].transform, false);
        //obj.transform.localPosition = new Vector3(-400 + 150*_hand.Count, -200, 0);
        return obj;
    }

    public void Play(Card target)
    {
        target.Effect();
        Discard(target._id);
    }

    private void DisplayHand()
    {
        for(byte i =0; i< _hand.Count; i++)
        {
            _hand.ElementAt(i).Value.transform.localPosition = new Vector3(-400 + 150 * i, -200, 0);
        }
    }
}
