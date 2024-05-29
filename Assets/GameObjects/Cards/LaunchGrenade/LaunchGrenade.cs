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

    byte _id;

    private void Awake()
    {
        _input = new CustomActions();
        _lineRenderer = GetComponent<LineRenderer>();
        _pathPoints = new List<Vector3>();
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _id = 0;
        while (manager.AddState("grenade"+_id.ToString(), EnterGrenadeState) == false) _id++;
    }

    void EnterGrenadeState()
    {
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        manager.SetLeftClickTo(FireGrenade);
        manager.SetRightClickTo(() => { manager.SetToDefult(); ClearPath(); });
        manager.SetHoverTo(Preview);
        GameObject.Find("Player").GetComponent<PlayerController>().ClearPath();
    }

    public override void Effect()
    {
        GameObject.Find("Player").GetComponent<PlayerManager>().SetToState("movement");
        base.Effect();
    }

    public override void ClickEvent()
    {
        GameObject.Find("Player").GetComponent<PlayerManager>().SetToState("grenade" + _id.ToString());
    }

    private void Preview()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, _clickableLayers))// Check if the hit point is on the NavMesh
        {
            // Crop the destination to the center of the target tile
            Vector3 alteredPos = hit.transform.position;
            alteredPos.y += 0.5f;

            // Calculate the path to the clicked point
            NavMeshPath path = new NavMeshPath();

            Vector3 playerPos = GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos;
            _grenadeInitVelocity = TrailCalculator.BellCurveInititialVelocity(playerPos, alteredPos, 10.0f);
            TrailCalculator.BellCurve(playerPos, _grenadeInitVelocity, ref _lineRenderer, out _pathPoints);
        }
    }

    protected void FireGrenade()
    {
        ClearPath();

        UnityEngine.Object GRENADE = Resources.Load("Grenade");
        GameObject grenade = (GameObject)Instantiate(GRENADE);
        grenade.GetComponent<Rigidbody>().transform.position = GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos;
        grenade.GetComponent<Rigidbody>().velocity = _grenadeInitVelocity;

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
    

    
}