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
        base.Init(CardType.SUPPORT, 2, 2, 60, stats);

        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState("TEMPLATE" + _id.ToString(), EnterState, ExitState) == false) _id++;
    }

    void EnterState()
    {
        PlayerManager manager = GI._PManFetcher();

        // Card range
        _selectableArea.SetSelectableEntites(false, true, true, true);
        _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, 0, 0);

        manager.SetLeftClickTo(Template);
        manager.SetRightClickTo(() => { 
            ExitState(); 
            GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault();
            if (_ghostHitbox != null)
                Destroy(_ghostHitbox);
        });
        manager.SetHoverTo(Template);
    }

    void ExitState()
    {
        _selectableArea.ResetSelectable();
        ClearPath();
    }

    void Template() { }

    public override void Effect()
    {
        /* Card Effect */

        base.Effect();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("TEMPLATE" + _id.ToString());
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

}