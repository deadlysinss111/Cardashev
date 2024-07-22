using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


// replace all TEMPLATE

public class TEMPLATE : Card
{

    /*
     * fields
     */

    private void Awake()
    {
        // Call the Card Initialization method with arguments as following (duration, maxLvl, goldValue, Stats)
        Dictionary<string, int> stats = new Dictionary<string, int>()
        {
            {"", 0}
        };
        
        /* stats fill there */
        base.Init("TEMPLATE", 2, 2, 60, stats);

        // Load here any model used by the card
    }

    // Called on card click
    override protected void EnterState()
    {
        // Card range
        _selectableArea.SetSelectableEntites(false, true, true, true);
        _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, 0, 0);

        base.EnterState();
    }

    override protected void ExitState()
    {
        base.ExitState();
    }


    override protected void OnLeftClick() 
    {
        /* Left Click */

        base.PlayCard();
    }

    public override void Effect()
    {
        /* Card Effect */

        base.Effect();
    }


    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

}