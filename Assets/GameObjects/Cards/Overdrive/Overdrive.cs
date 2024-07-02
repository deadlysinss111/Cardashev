using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

class Overdrive : Card
{
    private void Awake()
    {
        int[] stats = new int[1];
        stats[0] = 15;
        base.Init(2, 4, 80, stats, $"perform a jump to the target tile, generating a shockwave that deals {_stats[0]} dmg to ennemies in range");
    }

    public override void Effect()
    {
        base.Effect();
        GI._PStatFetcher().AddModifier(new StatManager.Modifier(StatManager.Modifier.ModifierType.Critical, 1, 10));
    }
}