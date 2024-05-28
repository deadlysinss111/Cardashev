using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

struct ANIMSTATES
{
    public const string IDLE = "IDLE";
    public const string WALK = "WALK";
}

public class PlayerController : MonoBehaviour
{
     CustomActions _input;

     NavMeshAgent _agent;
     Animator _animator;
     LineRenderer _lineRenderer;

    [Header("Movement")]
    [SerializeField]  ParticleSystem _clickEffect;

    [SerializeField]  LayerMask _clickableLayers;

     float _lookRotationSpeed = 8f;
     List<Vector3> _pathPoints;

    float _lastCalculatedWalkTime;

    bool _movementEnabled;

    // Initialization
     void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _lineRenderer = GetComponent<LineRenderer>();
        _pathPoints = new List<Vector3>();
        _input = new CustomActions();

        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        manager._virtualPos = _agent.transform.position;
        manager.AddState("movement", Preview);
    }

    // Handle click to visualize the path
     void Preview()
    {
        // Raycast to the clicked point
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, _clickableLayers))// Check if the hit point is on the NavMesh
        {

            // TEMP CODE -- Remove if I forgor
            // Baptiste :p
            Renderer rend = hit.transform.GetComponent<Renderer>();
            rend.material.color = Color.green;
            // \~ TEMP CODE END

            // Crop the destination to the center of the target tile
            Vector3 alteredPos = hit.transform.position;
            alteredPos.y += 0.5f;

            // Calculate the path to the clicked point
            NavMeshPath path = new NavMeshPath();

            // In the following snipet, the commented code are those that use not cropped positions
            //if (NavMesh.CalculatePath(_virtualPos, hit.point, NavMesh.AllAreas,  path))
            if (NavMesh.CalculatePath(GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos, alteredPos, NavMesh.AllAreas,  path))
            {
                TrailCalculator.DrawPath(path, ref _lineRenderer);
                _lastCalculatedWalkTime = GetPathTime(path);
            }

            // Instantiate click effect at the clicked point
            if (_clickEffect != null)
            {
                //Instantiate(_clickEffect, hit.point + new Vector3(0, 0.1f, 0), _clickEffect.transform.rotation);
                Instantiate(_clickEffect, alteredPos + new Vector3(0, 0.1f, 0), _clickEffect.transform.rotation);
            }

            GameObject.Find("Player").GetComponent<PlayerManager>().SetLeftClickTo(() => WaitForConfirmation(alteredPos));
        }
    }

    // Coroutine to wait for confirmation input
     void WaitForConfirmation(Vector3 destination)
    {
        // Update agent destination only when confirmed
        
        ClearPath();

        GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos = destination;

        Card moveCard = new Card();
        moveCard._trigger += () =>
        {
            _agent.destination = destination;
            StartCoroutine(UpdatePath());
        };
        moveCard._duration = _lastCalculatedWalkTime;

        if (GameObject.Find("Player").GetComponent<QueueComponent>().AddToQueue(moveCard) == false)
        {
            Debug.Log("error in movement card generation");
        }
    }
    

    // Coroutine to update the path as the agent moves
     IEnumerator UpdatePath()
    {
        // Wait for the agent to reach the destination
        while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
        {
            // Update the path as the agent moves
            Vector3 playerPosition = transform.position;
            while (_pathPoints.Count > 0 && Vector3.Distance(playerPosition, _pathPoints[0]) < 0.1f)
            {
                // Remove the first point if the player is close enough
                _pathPoints.RemoveAt(0);

                // Set positions for the line renderer
                _lineRenderer.positionCount = _pathPoints.Count;

                // Update the line renderer positions
                _lineRenderer.SetPositions(_pathPoints.ToArray());
            }
            yield return null;
        }
        ClearPath();
    }

    // Get the time to traverse the path
     float GetPathTime(NavMeshPath path)
    {
        // Calculate the time to traverse the path
        float time = 0;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            // Get the start and end points of the segment
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i + 1];

            // Add the time to traverse the segment
            time += Vector3.Distance(start, end) / _agent.speed;
        }
        return time;
    }

    // Clear the path from the line renderer
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

    // Update is called once per frame
    void Update()
    {
        //FaceTarget();
        SetAnimations();
    }

    // Rotate the player to face the target destination
     void FaceTarget()
    {
        // Calculate the direction to the target destination
        Vector3 direction = (_agent.destination - transform.position).normalized;

        // Rotate the player towards the target destination
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Smoothly rotate the player
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _lookRotationSpeed);
    }

    // Set animations based on agent velocity
     void SetAnimations()
    {
        if (_agent.velocity == Vector3.zero)
        {
            _animator.Play(ANIMSTATES.IDLE);
        }
        else
        {
            _animator.Play(ANIMSTATES.WALK);
        }
    }
}