using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

class Resilience : Card
{
    private void Awake()
    {
        Dictionary<string, int> stats = new Dictionary<string, int>()
        {
            {"shield", 100}
        };
        base.Init("Resilience", 2.5f, 4, 110, stats, $"Adds a second health bar of {stats["shield"]} points out of pure spite.");
    }

    public override void Effect()
    {
        base.Effect();
        GI._PStatFetcher().AddModifier(new StatManager.Modifier(StatManager.Modifier.ModifierType.Armor, _stats["shield"]));
    }
}