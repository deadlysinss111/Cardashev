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
    [SerializeField] LayerMask _clickableLayers;

    LineRenderer _lineRenderer;
    Vector3 _grenadeInitVelocity;
    GameObject _previwRadius;

    [SerializeField] GameObject _grenadePrefab;

    private void Awake()
    {
        _maxLv = 2;
        _goldValue = 60;
        _stats = new int[3];
        _lineRenderer = GetComponent<LineRenderer>();

        // Add a unique state + id to play the correct card and  not the first of its kind
        while (GI._PManFetcher().AddState("grenade"+_id.ToString(), EnterGrenadeState, ExitState) == false) _id++;

        UnityEngine.Object RADIUS = Resources.Load("RadiusPreview");
        _previwRadius = (GameObject)Instantiate(RADIUS);
        _previwRadius.SetActive(false);
    }

    void EnterGrenadeState()
    {
        PlayerManager manager = GI._PManFetcher();
        manager.SetLeftClickTo(FireGrenade);
        manager.SetRightClickTo(()=> { ExitState(); GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault(); });
        manager.SetHoverTo(Preview);
        _previwRadius.SetActive(true);
    }

    void ExitState()
    {
        _previwRadius.SetActive(false);
        ClearPath();
    }

    public override void Effect()
    {
        GameObject grenade = Instantiate(_grenadePrefab);
        grenade.GetComponent<Rigidbody>().transform.position = GI._PlayerFetcher().GetComponent<PlayerManager>()._virtualPos + new Vector3(0, 1, 0);
        grenade.GetComponent<Rigidbody>().velocity = _grenadeInitVelocity;

        base.Effect();
    }

    public override void ClickEvent()
    {
        GI._PManFetcher().SetToState("grenade" + _id.ToString());
    }

    private void Preview()
    {
        PlayerManager manager = GI._PManFetcher();
        // Crop the destination to the center of the target tile
        Vector3 alteredPos = manager._lastHit.transform.position;
        alteredPos.y += 0.5f;

        _previwRadius.transform.position = alteredPos;

        Vector3 playerPos = manager._virtualPos;
        _grenadeInitVelocity = TrajectoryToolbox.BellCurveInitialVelocity(playerPos, alteredPos, 10.0f);
        TrajectoryToolbox.BellCurve(playerPos, _grenadeInitVelocity, ref _lineRenderer);
    }

    protected void FireGrenade()
    {
        ClearPath();
        GI._PManFetcher().SetToDefault();
        // Trigger the card play event
        base.ClickEvent();
    }

    void ClearPath()
    {
        _lineRenderer.positionCount = 0;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

}