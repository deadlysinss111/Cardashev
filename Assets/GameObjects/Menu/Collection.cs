using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Collection : MonoBehaviour
{
    Canvas _canvas;

    List<GameObject> _currentUnlocked;
    List<GameObject> _currentLocked;

    // temp way to let the player have cards we'd like to have it in some external files)
    public static Dictionary<string, List<string>> _unlocked = new()
    {
        { "Dix", new List<string>(){
            "LaunchGrenadeCard",
            "PiercingShotCard",
            "SimpleShotCard",
            "JumpCard",
            "JumpAndShockwaveCard",
            "OverdriveCard",
            "ResilienceCard",
        } }
    };
    public static Dictionary<string, List<Tuple<string, List<UnlockCondition>>>> _locked = new()
    {
        { "Dix", new List<Tuple<string, List<UnlockCondition>>>()
            {
                new Tuple<string, List<UnlockCondition>>(
                    "Jeku",
                    new(){
                        new UnlockCondition("mouvements", 10),
                        new UnlockCondition("jumps", 15),
                    }
                )
            }
        }
    };

    void Load(string toLoad)
    {
        // Has no purpose other than resetting the dictionaries (for now)
        //_unlocked = new Dictionary<string, List<string>>();
        //_locked = new Dictionary<string, List<Tuple<string, UnlockCondition>>>();

        List<string> cards;

        _unlocked.TryGetValue(toLoad, out cards);
        foreach(string target in cards)
        {
            UnityEngine.Object CARD = Resources.Load(target);
            GameObject card = (GameObject)Instantiate(CARD);
            card.transform.SetParent(_canvas.transform, false);
            _currentUnlocked.Add(card);
        }

        List<Tuple<string, List<UnlockCondition>>> lockedCards;
        _locked.TryGetValue(toLoad, out lockedCards);

        // Fill the list with the name of the cards from the tuples
        cards.Clear();
        foreach (Tuple<string, List<UnlockCondition>> locked in lockedCards)
        {
            cards.Add(locked.Item1);
        }

        foreach(string target in cards)
        {
            UnityEngine.Object CARD = Resources.Load(target);
            GameObject card = (GameObject)Instantiate(CARD);
            card.transform.SetParent(_canvas.transform, false);
            // Either do something here to distinguish locked card
            _currentLocked.Add(card);
        }
    }

    /// <summary>
    /// Unlock a card of the playerId player
    /// </summary>
    /// <param name="playerId">The id of the player the card is for</param>
    /// <param name="card">The card to unlock</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static bool Unlock(string playerId, string card)
    {
        if (_locked.ContainsKey(playerId) == false)
        {
            throw new KeyNotFoundException("The playable character " + playerId + " doesn't exist.");
        }

        List<Tuple<string, List<UnlockCondition>>> cards;
        if (_locked.TryGetValue(playerId, out cards))
        {
            // tuple = cardName + Condition
            foreach (var tuple in cards)
            {
                if (tuple.Item1.Equals(card))
                {
                    _locked[playerId].Remove(tuple);
                    _unlocked[playerId].Add(card);
                    return true;
                }
            }
            return false;
        }
        return true;
    }
    /// <summary>
    /// Unlock a card by trying to find its corresponding player
    /// </summary>
    /// <param name="card">The card to unlock</param>
    /// <returns></returns>
    public static bool Unlock(string card)
    {
        List<Tuple<string, List<UnlockCondition>>> cards;
        foreach (var playerId in _locked.Keys)
        {
            if (_locked.TryGetValue(playerId, out cards))
            {
                // tuple = cardName + Condition
                foreach (var tuple in cards)
                {
                    if (tuple.Item1.Equals(card))
                    {
                        _locked[playerId].Remove(tuple);
                        _unlocked[playerId].Add(card);
                        return true;
                    }
                }
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if a card of the playerId player is unlocked
    /// </summary>
    /// <param name="playerId">The id of the player the card is from</param>
    /// <param name="card">The card to verify</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static bool IsUnlocked(string playerId, string card)
    {
        if (_locked.ContainsKey(playerId) == false)
        {
            throw new KeyNotFoundException("The playable character " + playerId + " doesn't exist.");
        }
        return _unlocked[playerId].Contains(card);
    }
    /// <summary>
    /// Checks if a card is unlocked, trying to find its corresponding player
    /// </summary>
    /// <param name="card">The card to verify</param>
    /// <returns></returns>
    public static bool IsUnlocked(string card)
    {
        foreach (var playerId in _unlocked.Keys)
        {
            if (_unlocked[playerId].Contains(card))
                return true;
        }
        return false;
    }

    // All below values for positions are totally random and meant to be changed later on
    void Display()
    {
        int x = 0;
        int y = 0;
        foreach(GameObject target in _currentUnlocked)
        {
            target.transform.localPosition = new Vector3(x, y, 0);
            x += 150;
            if(x >= 1350)
            {
                x = 0;
                y += 200;
            }
        }
        foreach(GameObject target in _currentLocked)
        {
            target.transform.localPosition = new Vector3(x, y, 0);
            // Either do something here to distinguish locked card
            x += 150;
            if (x >= 1350)
            {
                x = 0;
                y += 200;
            }
        }
    }
}
