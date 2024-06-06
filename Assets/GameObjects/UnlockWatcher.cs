using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to easily compare a specific stat to a wanted value
/// </summary>
public struct UnlockCondition
{
    public string _stat;
    int _successValue;
    bool _reverse;

    public UnlockCondition(string stat, int successValue, bool reverse=false)
    {
        _stat = stat;
        _successValue = successValue;
        _reverse = reverse;

        //GlobalStats.ListenToStat(stat, this.IsComplete);
    }

    public bool IsComplete()
    {
        //GlobalStats _stats = GameObject.FindGameObjectWithTag("Player").GetComponent<GlobalStats>();
        Debug.Log($"Checking if {_stat} ({GlobalStats.GetStat(_stat)}) is equal to {_successValue}...");
        bool cond = GlobalStats.GetStat(_stat) >= _successValue;
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
    private static Dictionary<string, UnlockCondition> unlockables;
    //GlobalStats _stats = GameObject.FindGameObjectWithTag("Player").GetComponent<GlobalStats>();

    private void Start()
    {
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
    static void AddUnlockable(Tuple<string, UnlockCondition> tuple)
    {
        GlobalStats.ListenToStat(tuple.Item2._stat, () => {
            Debug.Log("Listener Function");
            if (tuple.Item2.IsComplete() && Collection.IsUnlocked(tuple.Item1) == false)
            {
                Debug.Log($"Unlockable {tuple.Item1} get!");
                Collection.Unlock(tuple.Item1);
            }
            else
            {
                Debug.Log("You got NOTHING! You LOSE! GOOD DAY SIR!");
            }
        });
    }
}
