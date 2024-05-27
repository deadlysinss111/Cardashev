using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class LaunchGrenade : Card
{
    CustomActions _input;
    [SerializeField] LayerMask _clickableLayers;
    List<Vector3> _pathPoints;

    LineRenderer _lineRenderer;

    Vector3 _grenadeInitVelocity;

    private void Awake()
    {
        _input = new CustomActions();
        _lineRenderer = GetComponent<LineRenderer>();
        _pathPoints = new List<Vector3>();
        GameObject.Find("Player").GetComponent<PlayerManager>().AddState("grenade", Preview);
    }

    public override void Effect()
    {
        GameObject.Find("Player").GetComponent<PlayerManager>().SetToState("movement");
        base.Effect();
    }

    public override void ClickEvent()
    {
        GameObject.Find("Player").GetComponent<PlayerManager>().SetToState("grenade");
    }

    private void Preview()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, _clickableLayers))// Check if the hit point is on the NavMesh
        {
            // Crop the destination to the center of the target tile
            Vector3 alteredPos = hit.transform.position;
            alteredPos.y += 0.5f;

            // Cancel the previous confirmation waiting coroutine
            if (GameObject.Find("Player").GetComponent<PlayerManager>()._waitForConfirmationCoroutine != null)
            {
                StopCoroutine(GameObject.Find("Player").GetComponent<PlayerManager>()._waitForConfirmationCoroutine);
                ClearPath();
            }

            // Calculate the path to the clicked point
            NavMeshPath path = new NavMeshPath();

            Vector3 playerPos = GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos;
            _grenadeInitVelocity = TrailCalculator.BellCurveInititialVelocity(playerPos, alteredPos, 10.0f);
            TrailCalculator.BellCurve(playerPos, _grenadeInitVelocity, ref _lineRenderer, out _pathPoints);

            // Start waiting for confirmation
            GameObject.Find("Player").GetComponent<PlayerManager>()._waitForConfirmationCoroutine = StartCoroutine(WaitForConfirmation(hit.point));
        }
    }

    IEnumerator WaitForConfirmation(Vector3 destination)
    {
        // Wait for the confirmation input
        while (_input.Main.Confirm.triggered == false)
        {
            yield return null;
        }

        // Launche the grenade only when confirmed
        ClearPath();

        Object grenadePrefab = Resources.Load("Grenade");
        GameObject grenade = (GameObject)Instantiate(grenadePrefab);
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