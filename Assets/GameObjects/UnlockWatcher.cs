using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UnlockCondition
{
    string _stat;
    int _successValue;
    bool _reverse;

    UnlockCondition(string stat, int successValue, bool reverse=false)
    {
        _stat = stat;
        _successValue = successValue;
        _reverse = reverse;
    }

    bool IsComplete()
    {
        //GlobalStats _stats = GameObject.FindGameObjectWithTag("Player").GetComponent<GlobalStats>();
        bool cond = GlobalStats.GetStat(_stat) >= _successValue;
        if (_reverse)
            cond = !cond;
        return cond;
    }
}

public static class UnlockWatcher
{
    //GlobalStats _stats = GameObject.FindGameObjectWithTag("Player").GetComponent<GlobalStats>();
    static void AddUnlockable()
    {

    }
}
