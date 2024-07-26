using System;
using System.Collections.Generic;
using UnityEngine;

public class HeavyCover : Card
{

    /*
     * fields
     */


    private void Awake()
    {
        // Call the Card Initialization method with arguments as following (duration, maxLvl, goldValue, Stats)
        Dictionary<string, int> stats = new Dictionary<string, int>();
        /* stats fill there */
        base.Init(2, 2, 60, stats);

        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState("HeavyCover" + _id.ToString(), EnterState, ExitState) == false) _id++;
    }

    void EnterState()
    {
        ActivateCover();
        GI._PManFetcher().SetToDefault();
    }

    void ExitState()
    {
    }

    void ActivateCover()
    {
        GameObject player = GI._PlayerFetcher();
        player.GetComponent<CoverManager>().EnableFullCover(1000);
        Effect();
    }

    public override void Effect()
    {
        /* Card Effect */

        base.Effect();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("HeavyCover" + _id.ToString());
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

}