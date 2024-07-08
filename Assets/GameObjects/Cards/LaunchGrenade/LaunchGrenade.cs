using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class LaunchGrenade : Card
{
    Vector3 _grenadeInitVelocity;
    Vector3 _grenadeOrigine;
    GameObject _previwRadius;

    GameObject _grenadePrefab;

    private void Awake()
    {
        // Call the Card Initialization method with arguments as following (duration, maxLvl, goldValue, Stats)
        int[] stats = new int[1];
        stats[0] = 15;
        string desc = $"launch a grenade exploding on ground contact, dealing {stats[0]} to enemies in range of explosion";
        base.Init(1, 2, 60, stats, desc);
        
        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState("grenade"+_id.ToString(), EnterGrenadeState, ExitState) == false) _id++;

        _grenadePrefab = (GameObject)Resources.Load("Grenade");

        if (TryGetComponent(out _selectableArea) == false)
            _selectableArea = gameObject.AddComponent<SelectableArea>();
    }

    void EnterGrenadeState()
    {
        PlayerManager manager = GI._PManFetcher();
        _selectableArea.SetSelectableEntites(false, true, true, true);
        _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, 15, 8);

        manager.SetLeftClickTo(FireGrenade);
        manager.SetRightClickTo(()=> { ExitState(); GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault(); });
        manager.SetHoverTo(Preview);
        _previwRadius.SetActive(true);
        GI.UpdateCursors("Bomb", (byte)(GI.CursorRestriction.S_TILES));
        GI.UpdateCursorsInverted("Cross", (byte)(GI.CursorRestriction.S_TILES));
    }

    void ExitState()
    {
        _previwRadius.SetActive(false);
        _selectableArea.ResetSelectable();
        //ClearPath();
    }

    public override void Effect()
    {
        GameObject grenade = Instantiate(_grenadePrefab);
        grenade.GetComponent<Rigidbody>().transform.position = _grenadeOrigine + new Vector3(0, 5, 0);
        grenade.GetComponent<GrenadeScript>()._velocity = _grenadeInitVelocity;
        grenade.GetComponent<GrenadeScript>()._dmg = _stats[0];
        grenade.GetComponent<GrenadeScript>()._origin = _grenadeOrigine + new Vector3(0, 1, 0);

        base.Effect();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("grenade" + _id.ToString());
    }

    private void Preview()
    {
        PlayerManager manager = GI._PManFetcher();
        // Crop the destination to the center of the target tile
        Vector3 alteredPos = manager._lastHit.transform.position;
        alteredPos.y += 0.5f;
        if (_selectableArea.CheckForSelectableTile(alteredPos) == false)
        {
            ClearPath();
            _previwRadius.SetActive(false);
            return;
        }
        _previwRadius.SetActive(true);

        _previwRadius.transform.position = alteredPos;

        _grenadeOrigine = manager._virtualPos;
        _grenadeInitVelocity = TrajectoryToolbox.BellCurveInitialVelocity(_grenadeOrigine + new Vector3(0, 1, 0), alteredPos, 10.0f);
        TrajectoryToolbox.BellCurve(_grenadeOrigine + new Vector3(0, 1, 0), _grenadeInitVelocity, ref _lineRenderer);
    }

    protected void FireGrenade()
    {
        //ClearPath();
        _selectableArea.ResetSelectable();
        GI._PManFetcher().SetToDefault();
        // Trigger the card play event
        base.PlayCard();
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

    public override void OnLoad()
    {
        UnityEngine.Object RADIUS = Resources.Load("RadiusPreview");
        _previwRadius = (GameObject)Instantiate(RADIUS);
        _previwRadius.SetActive(false);
    }
    public override void OnUnload()
    {
        Destroy(_previwRadius);
    }

}