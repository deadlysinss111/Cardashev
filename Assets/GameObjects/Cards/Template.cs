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
        int[] stats = new int[0];
        /* stats fill there */
        base.Init(2, 2, 60, stats);

        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState(_name + _id.ToString(), EnterState, ExitState) == false) _id++;

        if (TryGetComponent(out _selectableArea) == false)
            _selectableArea = gameObject.AddComponent<SelectableArea>();
        else
            _selectableArea = GetComponent<SelectableArea>();

        // Load here any model used by the card
    }

    // Called on card click
    void EnterState()
    {
        PlayerManager manager = GI._PManFetcher();

        // Card range
        _selectableArea.SetSelectableEntites(false, true, true, true);
        _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, 0, 0);

        manager.SetLeftClickTo(OnLeftClick);
        manager.SetRightClickTo(() => { 
            ExitState(); 
            GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault();
            if (_ghostHitbox != null)
                Destroy(_ghostHitbox);
        });
        manager.SetHoverTo(Preview);
    }

    void ExitState()
    {
        _selectableArea.ResetSelectable();
        ClearPath();
    }

    void OnLeftClick() { }

    public override void Effect()
    {
        /* Card Effect */

        base.Effect();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState(_name + _id.ToString());
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

}