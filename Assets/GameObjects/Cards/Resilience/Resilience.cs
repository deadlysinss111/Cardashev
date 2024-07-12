using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

class Resilience : Card
{
    private void Awake()
    {
        int[] stats = new int[1];
        stats[0] = 100;
        base.Init(2.5f, 4, 110, stats, $"Adds a second health bar of {stats[0]} points out of pure spite.");
    }

    public override void Effect()
    {
        base.Effect();
        GI._PStatFetcher().AddModifier(new StatManager.Modifier(StatManager.Modifier.ModifierType.Armor, _stats[0]));
    }
}