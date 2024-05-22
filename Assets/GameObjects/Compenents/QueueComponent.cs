using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class QueueComponent : MonoBehaviour
{

    private Queue<Card> _queue = new Queue<Card>();
    private Card _activeCard;

    void Update()
    {
        // If N is pressed, create a test card and try to add it to the queue
        /*if (Input.GetKeyDown(KeyCode.N))
        {
            //Card c = new("Sword of Light", "A huge and heavy railgun made for spatial airships. As of now, only a robot has managed to weild it... And you, for some reason.", 2f);
            AddToQueue(c);
        }*/

        // Handle making a card from the queue the "active" one
        if (IsCurrentCardEmpty() && _queue.Count > 0)
        {
            _activeCard = _queue.Dequeue();
            _activeCard.Effect();
        }

        if (IsCurrentCardEmpty()) return;
        
        // Handle removing the active card once its time is up
        if (_activeCard.GetRemainingTime() <= 0)
        {
            //Debug.Log("The card " + _activeCard.name + " got casted!!");
            // It has been used so reset the current card
            _activeCard = null;
        }
        else
        {
            
        }

    }

    /// <summary>
    /// Checks if _activeCard is equal to its default values, which is the equivalent of null for structs
    /// </summary>
    /// <returns></returns>
    bool IsCurrentCardEmpty()
    {
        return _activeCard is null;
    }

    /// <summary>
    /// Adds the card c to the queue if possible
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public bool AddToQueue(Card c)
    {
        //BetterDebug.Log(Epoch.GetCurrentTimeFloat(), card.name, queue);
        if (AllowCard(c))
        {
            _queue.Enqueue(c);
            return true;
        }
        Debug.Log("New Card refused!");
        return false;
    }

    /// <summary>
    /// Get the total amount of seconds all the cards in the queue takes
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Checks if the total time of the queue goes beyond its limit
    /// </summary>
    /// <returns></returns>
    bool IsQueueFull()
    {
        return TotalQueueTime() >= 10f;
    }

    /// <summary>
    /// Checks if the card c can be added to the queue
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    bool AllowCard(Card c)
    {
        return !IsQueueFull() && TotalQueueTime() + c._duration <= 10f;
    }
}
