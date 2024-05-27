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

    public Card()
    {
        _trigger += ()=>Effect();
    }

    public virtual void Effect()
    {
        _endingTime = Time.time + _duration;
    }

    public float GetRemainingTime()
    {
        return _endingTime - Time.time;
    }

    private void OnMouseDown()
    {
        ClickEvent();
    }

    public virtual void ClickEvent()
    {
        GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<DeckManager>().Play(this);
    }

}
