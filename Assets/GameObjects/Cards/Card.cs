using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Card : MonoBehaviour
{
    public string _name;
    public string _description;
    public float _duration;
    public float _endingTime;
    public int[] _stats;
    public int _goldValue;
    public Action _onDiscard;
    public Action _trigger;
    public Action _clickEffect;

    public int _currLv = 1;
    public int _maxLv = 3;

    public Card()
    {
        _trigger += ()=>Effect();
        _clickEffect = ClickEvent;
    }

    public virtual void Effect()
    {
        _endingTime = Time.time + _duration;
    }
    public virtual void Effect(GameObject go)
    {
        Effect();
    }

    public float GetRemainingTime()
    {
        return _endingTime - Time.time;
    }

    private void OnMouseDown()
    {
        _clickEffect();
    }

    public virtual void ClickEvent()
    {
        GameObject.Find("Player").GetComponent<DeckManager>().Play(this);
    }

    public void SetToCollectible(Func<bool> func)
    {
        _clickEffect = ()=> { if (func()) { PlayerManager._deck.Add(this); } };
    }

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
