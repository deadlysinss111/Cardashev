using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;
using static UnityEngine.GraphicsBuffer;

public class EnemyDeckManager : MonoBehaviour
{
    protected List<EnemyCard> _cardPool;

    Card _activeCard;
    float _activeCardEndTime;

    void Start()
    {
        List<string> toLoad = GetComponent<BasicEnemyHandler>().GetDeck();
        _cardPool = new List<EnemyCard>();
        foreach (string name in toLoad)
        {
            EnemyCard card;
            EnemyEncyclopedia._enemyCardBook.TryGetValue(name, out card);
            _cardPool.Add(card);
        }
    }


    public float Play()
    {
        int rdm = UnityEngine.Random.Range(0, _cardPool.Count);
        _cardPool[rdm]._effect(gameObject);
        return _cardPool[rdm]._duration;
    }
}
