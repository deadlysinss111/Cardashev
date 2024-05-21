using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// Temporary Card structure
struct Card
{
    public string id;
    public string name;
    public string description;
    public float duration;
    private float endingTime;

    public Card(string id, string name="Name", string description="Sample Description", float duration=2f)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.duration = duration;
        this.endingTime = float.MinValue; //temporary value to signifies the card is not active
    }

    public float getRemainingTime()
    {
        return endingTime - Time.time;
    }

    /// <summary>
    /// Sets up the card's endingTime value so that its time left to act can be counted in-game
    /// </summary>
    public void StartTimer()
    {
        this.endingTime = Time.time + this.duration;
    }
}

public class QueueComponent : MonoBehaviour
{

    private Queue<Card> _queue = new Queue<Card>();
    private Card _currentCard;

    void Update()
    {
        // If N is pressed, create a test card and try to add it to the queue
        if (Input.GetKeyDown(KeyCode.N))
        {
            Card c = new("railgun", "Sword of Light", "A huge and heavy railgun made for spacial airships. As of now, only a robot has managed to weild it... And you, for some reason.", 2f);
            AddToQueue(c);
        }

        // Handle making a card from the queue the "active" one
        if (IsCurrentCardEmpty() && _queue.Count > 0)
        {
            _currentCard = _queue.Dequeue();
            _currentCard.StartTimer();
        }

        if (!IsCurrentCardEmpty())
        {
            // Handle removing the active card once its time is up
            if (_currentCard.getRemainingTime() <= 0)
            {
                Debug.Log("The card " + _currentCard.name + " got casted!!");
                // It has been used so reeset the current card
                _currentCard = default(Card);
            }
        }

    }

    /// <summary>
    /// Checks if _currentCard is equal to its default values, which is the equivalent of null for structs
    /// </summary>
    /// <returns></returns>
    bool IsCurrentCardEmpty()
    {
        return _currentCard.Equals(default(Card));
    }

    /// <summary>
    /// Adds the card c to the queue if possible
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    bool AddToQueue(Card c)
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
    float TotalQueueTime()
    {
        float totalTime = 0;
        if (!IsCurrentCardEmpty())
        {
            totalTime += _currentCard.getRemainingTime();
        }

        foreach (Card c in _queue)
        {
            totalTime += c.duration;
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
        return !IsQueueFull() && TotalQueueTime() + c.duration <= 10f;
    }
}
