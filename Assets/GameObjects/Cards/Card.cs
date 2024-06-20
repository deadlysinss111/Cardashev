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
    // TODO make them into protected with properties
    public string _name;
    public string _description;
    public float _duration;
    public int[] _stats;
    public int _goldValue;

    // Utilities
    public float _cardEndTimestamp;
    public byte _id = 0;
    protected SelectableArea _selectableArea;
    protected LineRenderer _lineRenderer;

    [SerializeField] protected LayerMask _clickableLayers;

    // Actions
    public Action _onDiscard;       // Called when the card's duration reached 0 after activation
    public Action _trigger;         // Called when the Queue sets it to be active
    public Action _clickEffect;     // Called when the card is clicked in the HUD

    // Level related
    public int _currLv = 1;
    public int _maxLv = 3;
    

    /*
     METHODS
    */
    // Constructor & Init
    public Card()
    {
        _trigger += () => Effect();
        _clickEffect = ClickEvent;
    }

    protected void Init(byte duration, byte maxLvl, int goldValue, int[] stats)
    {
        _duration = duration;
        _maxLv = maxLvl;
        _goldValue = goldValue;
        _stats = stats;

        if(TryGetComponent(out LineRenderer renderer))
            _lineRenderer = renderer;
        else
            _lineRenderer = null;
    }

    // ~~~(> GETTERS
    public float GetRemainingTime()
    {
        return _cardEndTimestamp - Time.time;
    }

    // In shop & rewards behaviour
    // TODO => move it to its own, new class
    public void SetToCollectible(Func<bool> func)
    {
        _clickEffect = () => { if (func()) CurrentRunInformations.AddCardsToDeck(new List<Card> { this }); };
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
        GI._PlayerFetcher().GetComponent<DeckManager>().Play(this);
    }

    protected void ClearPath()
    {
        _lineRenderer.positionCount = 0;
    }


    // ------
    // UPGRADE METHODS
    // ------
    public virtual void OnUpgrade()
    {
        for (int i = 0; i < _stats.Length; i++)
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