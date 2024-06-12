using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to easily compare a specific stat to a wanted value
/// </summary>
public struct UnlockCondition
{
    public string _statName;
    int _successValue;
    bool _reverse;

    public UnlockCondition(string stat, int successValue, bool reverse=false)
    {
        _statName = stat;
        _successValue = successValue;
        _reverse = reverse;

        //GlobalStats.ListenToStat(stat, this.IsComplete);
    }

    public bool IsComplete()
    {
        //GlobalStats _stats = GameObject.FindGameObjectWithTag("Player").GetComponent<GlobalStats>();
        //Debug.Log($"Checking if {_stat} ({GlobalStats.GetStat(_stat)}) is equal to {_successValue}...");
        bool cond = GlobalStats.GetStat(_statName) >= _successValue;
        if (_reverse)
            cond = !cond;
        return cond;
    }
}

/// <summary>
/// 
/// </summary>
public class UnlockWatcher : MonoBehaviour
{
    private static Dictionary<string, List<UnlockCondition>> _unlockables;
    //GlobalStats _stats = GameObject.FindGameObjectWithTag("Player").GetComponent<GlobalStats>();

    private void Start()
    {
        _unlockables = new();
        foreach (var cardList in Collection._locked)
        {
            foreach(var card in cardList.Value)
            {
                AddUnlockable(card);
            }
        }
    }

    /// <summary>
    /// Defines a lambda function to listen to a specific stat
    /// </summary>
    /// <param name="tuple">The unlockable to define a listener for</param>
    static void AddUnlockable(Tuple<string, List<UnlockCondition>> tuple)
    {
        foreach (UnlockCondition cond in tuple.Item2)
        {
            GlobalStats.ListenToStat(cond._statName, () => {
                //Debug.Log("Listener Function");
                if (AreCondComplete(tuple.Item1) && Collection.IsUnlocked(tuple.Item1) == false)
                {
                    Debug.Log($"Unlockable {tuple.Item1} get!");
                    Collection.Unlock(tuple.Item1);
                }
                else
                {
                    //Debug.Log("You got NOTHING! You LOSE! GOOD DAY SIR!");
                }
            });
            if (false == _unlockables.ContainsKey(tuple.Item1))
            {
                _unlockables.Add(tuple.Item1, new List<UnlockCondition>());
            }
            _unlockables[tuple.Item1].Add(cond);
        }
    }

    public static bool AreCondComplete(string card)
    {
        foreach (var cond in _unlockables[card])
        {
            if (false == cond.IsComplete())
                return false;
        }
        return true;
    }
}
