using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

class Overdrive : Card
{
    public override void Effect()
    {
        base.Effect();
        GI._PlayerFetcher().GetComponent<StatManager>().AddModifier(new StatManager.Modifier(StatManager.Modifier.ModifierType.Critical, 1, 10));
    }
}