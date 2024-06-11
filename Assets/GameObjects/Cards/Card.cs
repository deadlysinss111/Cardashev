using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

public class Card : MonoBehaviour
{
    /*
     FIELDS
    */
    // Card identity
    public string _name;
    public string _description;
    public float _duration;
    public int[] _stats;
    public int _goldValue;

    // Utilities
    public float _cardEndTimestamp;

    // Actions, idfk
    public Action _onDiscard;       // Called when the card's duration reached 0 after activation
    public Action _trigger;         // Called when the Queue sets it to be active
    public Action _clickEffect;     // Called when the card is clicked in the HUD

    // Level related
    public int _currLv = 1;
    public int _maxLv = 3;


    /*
     METHODS
    */
    // Constructor
    public Card()
    {
        _trigger += ()=>Effect();
        _clickEffect = ClickEvent;
    }

    // ~~~(> GETTERS
    public float GetRemainingTime()
    {
        return _cardEndTimestamp - Time.time;
    }

    // ~~~(> SETTERS
    // ??????
    public void SetToCollectible(Func<bool> func)
    {
        _clickEffect = () => {   if (func())  PlayerManager._deck.Add(this);   };
    }


    // -----
    // ACTIONS, FUNCS AND CALLBACKS
    // -----
    public virtual void Effect()
    {
        _cardEndTimestamp = Time.time + _duration;
    }
    public virtual void Effect(GameObject go)
    {
        Effect();
    }

    private void OnMouseDown()
    {
        _clickEffect();
    }

    public virtual void ClickEvent()
    {
        GameObject.Find("Player").GetComponent<DeckManager>().Play(this);
    }


    // ------
    // UPGRADE METHODS
    // ------
    public virtual void OnUpgrade()
    {
        for (int i=0; i< _stats.Length; i++)
        {
            _stats[i] += 2 * _currLv;
        }
    }

    public bool CanUpgrade()
    {
        return _currLv < _maxLv;
    }

    public bool Upgrade()
    {
        if (CanUpgrade())
        {
            _currLv++;
            OnUpgrade();
            return true;
        }
        return false;
    }
}
