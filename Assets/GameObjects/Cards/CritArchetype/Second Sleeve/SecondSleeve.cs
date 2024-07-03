using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;


// replace all TEMPLATE

public class SecondSleeve : Card
{
    [SerializeField] Vignette lol;

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
        while (PlayerManager.AddState("SecondSleeve" + _id.ToString(), EnterState, ExitState) == false) _id++;
    }

    void EnterState()
    {
        PlayerManager manager = GI._PManFetcher();

        // Card range
        _selectableArea.SetSelectableEntites(false, true, true, true);
        _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, 0, 0);

        manager.SetLeftClickTo(() => { ClearPath(); _selectableArea.ResetSelectable(); GI._PManFetcher().SetToDefault(); base.PlayCard(); });
        manager.SetRightClickTo(() => { ExitState(); GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault(); });
        manager.SetHoverTo(DisplayRange);
    }

    void ExitState()
    {
        _selectableArea.ResetSelectable();
        ClearPath();
    }

    void DisplayRange()
    {
        // Possibility to explore
        lol.intensity.min = 0.35f;
        lol.intensity.min = 0.36f;
    }

    public override void Effect()
    {


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