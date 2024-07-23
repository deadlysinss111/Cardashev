using System;
using TMPro;
using UnityEngine;

class Overdrive : Card
{
    private void Awake()
    {
        int[] stats = new int[1];
        stats[0] = 10;
        base.Init(2, 4, 80, stats, $"Activate the Critical status for {stats[0]} seconds");
    }

    public override void Effect()
    {
        base.Effect();
        GI._PStatFetcher().AddModifier(new StatManager.Modifier(StatManager.Modifier.ModifierType.Critical, 1, _stats[0]));
    }
}