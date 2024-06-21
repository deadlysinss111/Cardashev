using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueComponent : MonoBehaviour
{
    private Queue<Card> _queue = new Queue<Card>();
    private Card _activeCard;
    QueueUI _ui;
    readonly private float _maxTimeBuffer = 15.0f;

    public float _MaxTimeBuffer
    {
        get { return _maxTimeBuffer; }
    }

    private void Start()
    {
        _ui = GameObject.Find("QueueBar").GetComponent<QueueUI>();
    }

    void Update()
    {
        if (IsCurrentCardEmpty() && _queue.Count > 0)
        {
            _activeCard = _queue.Dequeue();
            _activeCard._trigger.Invoke();
        }
        else if (IsCurrentCardEmpty()) return;

        if (_activeCard.GetRemainingTime() <= 0)
        {
            _activeCard = null;
        }
    }

    public bool IsCurrentCardEmpty()
    {
        return _activeCard is null;
    }

    public bool AddToQueue(Card c)
    {
        c._actionColor = GenerateRandomColor();
        if (AllowCard(c))
        {
            _queue.Enqueue(c);
            _ui.AddSegment(c._duration, c._actionColor);
            return true;
        }
        Debug.Log("New Card refused!");
        return false;
    }

    public float TotalQueueTime()
    {
        float totalTime = 0;
        if (!IsCurrentCardEmpty())
        {
            totalTime += _activeCard.GetRemainingTime();
        }

        foreach (Card c in _queue)
        {
            totalTime += c._duration;
        }

        return totalTime;
    }

    bool IsQueueFull()
    {
        return TotalQueueTime() >= _maxTimeBuffer;
    }

    bool AllowCard(Card c)
    {
        return !IsQueueFull() && TotalQueueTime() + c._duration <= _maxTimeBuffer && c._duration!=0;
    }

    public Queue<Card> GetQueue()
    {
        return _queue;
    }

    public Card GetActiveCard()
    {
        return _activeCard;
    }

    public float GetCumulativeTimeOfPreviousCards()
    {
        float cumulativeTime = 0;
        foreach (Card c in _queue)
        {
            if (c == _activeCard) break;
            cumulativeTime += c._duration;
        }
        return cumulativeTime;
    }

    Color GenerateRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}
