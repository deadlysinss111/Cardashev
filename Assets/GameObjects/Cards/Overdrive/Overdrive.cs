using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Card;

class Overdrive : Card
{
    private void Awake()
    {
        Dictionary<string, int> stats = new Dictionary<string, int>()
        {
            {"duration", 10}
        };
        _id = 0;
        foreach(Transform t in GI._deckContainer.transform)
        {
            if (t.TryGetComponent<Overdrive>(out _))
                ++_id;
        }

        base.Init(CardType.SUPPORT, 2, 4, 80, stats, $"Activate the Critical status for {stats["duration"]} seconds");
    }

    public override void Effect()
    {
        base.Effect();
        GI._PStatFetcher().AddModifier(new StatManager.Modifier(StatManager.Modifier.ModifierType.Critical, 1, _stats["duration"]));
    }
}