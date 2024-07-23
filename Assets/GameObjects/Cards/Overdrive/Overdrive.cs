using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

class Overdrive : Card
{
    private void Awake()
    {
        Dictionary<string, int> stats = new Dictionary<string, int>()
        {
            {"duration", 10}
        };

        base.Init(2, 4, 80, stats, $"Activate the Critical status for {stats["duration"]} seconds");
    }

    public override void Effect()
    {
        base.Effect();
        GI._PStatFetcher().AddModifier(new StatManager.Modifier(StatManager.Modifier.ModifierType.Critical, 1, _stats["duration"]));
    }
}