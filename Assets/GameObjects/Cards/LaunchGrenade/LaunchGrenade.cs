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
    CustomActions _input;
    [SerializeField] LayerMask _clickableLayers;
    List<Vector3> _pathPoints;

    LineRenderer _lineRenderer;

    Vector3 _grenadeInitVelocity;

    GameObject _previwRadius;

    byte _id;

    private void Awake()
    {
        _maxLv = 2;
        _stats = new int[3];
        _input = new CustomActions();
        _lineRenderer = GetComponent<LineRenderer>();
        _pathPoints = new List<Vector3>();
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _id = 0;
        while (manager.AddState("grenade"+_id.ToString(), EnterGrenadeState, ExitState) == false) _id++;
        UnityEngine.Object RADIUS = Resources.Load("RadiusPreview");
        _previwRadius = (GameObject)Instantiate(RADIUS);
        _previwRadius.SetActive(false);
    }

    void EnterGrenadeState()
    {
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
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
        UnityEngine.Object GRENADE = Resources.Load("Grenade");
        GameObject grenade = (GameObject)Instantiate(GRENADE);
        grenade.GetComponent<Rigidbody>().transform.position = GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos + new Vector3(0, 1, 0);
        grenade.GetComponent<Rigidbody>().velocity = _grenadeInitVelocity;

        base.Effect();
    }

    public override void ClickEvent()
    {
        GameObject.Find("Player").GetComponent<PlayerManager>().SetToState("grenade" + _id.ToString());
    }

    private void Preview()
    {
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        // Crop the destination to the center of the target tile
        Vector3 alteredPos = manager._lastHit.transform.position;
        alteredPos.y += 0.5f;

        _previwRadius.transform.position = alteredPos;

        Vector3 playerPos = manager._virtualPos;
        _grenadeInitVelocity = TrailCalculator.BellCurveInititialVelocity(playerPos, alteredPos, 10.0f);
        TrailCalculator.BellCurve(playerPos, _grenadeInitVelocity, ref _lineRenderer, out _pathPoints);
    }

    protected void FireGrenade()
    {
        ClearPath();
        GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault();
        // Trigger the card play event
        base.ClickEvent();
    }

    void ClearPath()
    {
        _lineRenderer.positionCount = 0;
    }

    // Enable input actions
    void OnEnable()
    {
        _input.Enable();
    }

    // Disable input actions
    void OnDisable()
    {
        _input.Disable();
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

}