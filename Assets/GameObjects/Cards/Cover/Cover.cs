using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Cover : Card
{

    /*
     * fields
     */

    GameObject _cover;

    private void Awake()
    {
        // Call the Card Initialization method with arguments as following (duration, maxLvl, goldValue, Stats)
        Dictionary<string, int> stats = new Dictionary<string, int>()
        {
            {"health", 50},
            {"duration", 6}
        };
        /* stats fill there */
        base.Init(2, 4, 60, stats, $"Summon a protection tanking {stats["health"]} dmg for {stats["duration"]}");

        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState("Cover" + _id.ToString(), EnterState, ExitState) == false) _id++;

        if (gameObject.TryGetComponent(out _rotationArrow) == false)
        {
            _rotationArrow = gameObject.AddComponent<RotationSelectArrow>();
        }
    }

    void EnterState()
    {
        PlayerManager manager = GI._PManFetcher();

        // Card range
        _rotationArrow.SetArrow(true);

        manager.SetLeftClickTo(ActivateCover);
        manager.SetRightClickTo(ExitState);
        manager.SetHoverTo(() => { });
    }

    void ExitState()
    {
        //_selectableArea.ResetSelectable();
        //ClearPath();
        _rotationArrow.SetArrow(false);
    }

    void ActivateCover()
    {
        base.PlayCard();
        GI._PManFetcher().SetToDefault();
    }

    public override void Effect()
    {
        /* Card Effect */
        GameObject player = GI._PlayerFetcher();
        player.GetComponent<CoverManager>().EnableCover(_stats["health"], _rotationArrow.GetRotation(), _stats["duration"]);
        player.GetComponent<PlayerController>()._moveMult = 0.3f;
        base.Effect();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("Cover" + _id.ToString());
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

    public override void OnLoad()
    {
        _cover = GI._PlayerFetcher().transform.Find("Cover").gameObject;
    }

}