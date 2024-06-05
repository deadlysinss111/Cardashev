using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
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
     LineRenderer _previewLineRenderer;

    [Header("Movement")]
    [SerializeField]  ParticleSystem _clickEffect;

    [SerializeField]  LayerMask _clickableLayers;

    float _lookRotationSpeed = 8f;
    List<Vector3> _pathPoints;

    float _lastCalculatedWalkTime;

    bool _movementEnabled;
    Vector3 _virtualDestination;

    List<Vector3[]> _paths;
    Vector3[] _previewPath;

    // Initialization
    void Awake()
    {
        // Loads in the fields useful data and references
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _lineRenderer = GetComponent<LineRenderer>();
        _previewLineRenderer = GameObject.Find("RoomAnchor").GetComponent<LineRenderer>();
        _pathPoints = new List<Vector3>();
        _input = new CustomActions();
        _paths = new List<Vector3[]>();

        // Loading in PlayerManager a new state and its Action to change what the controls will do
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        manager._virtualPos = _agent.transform.position;
        manager.AddState("movement", EnterMovementState, ExitState);
    }

    void EnterMovementState()
    {
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        manager.SetLeftClickTo(ApplyMovement);
        manager.SetRightClickTo(() => { });
        manager.SetHoverTo(Preview);
    }

    void ExitState()
    {
        ClearPath();
    }

     // Handle click to visualize the path
     void Preview()
    {
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        // Crop the destination to the center of the target tile
        Vector3 alteredPos = manager._lastHit.transform.position;
        alteredPos.y += 0.5f;

        // Calculate the path to the clicked point
        NavMeshPath path = new NavMeshPath();

        // In the following snipet, the commented code are those that use not cropped positions
        //if (NavMesh.CalculatePath(_virtualPos, hit.point, NavMesh.AllAreas,  path))
        if (NavMesh.CalculatePath(manager._virtualPos, alteredPos, NavMesh.AllAreas,  path))
        {
            _previewPath = path.corners;
            TrailCalculator.DrawPath(_previewPath, ref _previewLineRenderer);
            _lastCalculatedWalkTime = GetPathTime(path);
        }

        // Instantiate click effect at the clicked point
        if (_clickEffect != null)
        {
            //Instantiate(_clickEffect, hit.point + new Vector3(0, 0.1f, 0), _clickEffect.transform.rotation);
            Instantiate(_clickEffect, alteredPos + new Vector3(0, 0.1f, 0), _clickEffect.transform.rotation);
        }

        _virtualDestination = alteredPos;
    }

    // Coroutine to wait for confirmation input
     void ApplyMovement()
     {
        // We keep trail of the preview
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos, _virtualDestination, NavMesh.AllAreas, path);
        _paths.Add(path.corners);
        TrailCalculator.DrawPath(_paths, ref _lineRenderer);

        //ClearPath();

        GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos = _virtualDestination;

        // We stock the information so that the closure knows what to take
        Vector3 vect = _virtualDestination;

        List<Vector3> slicedPath = new List<Vector3>();
        
        for(int i =0; i<path.corners.Length-1; i++)
        {
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i+1];
            int segments = Mathf.CeilToInt(Vector3.Distance(start, end) / 0.1f); // Adjust segment length as needed

            for (int j = 0; j <= segments; j++)
            {
                // Calculate the point along the segment
                float t = (float)j / segments;

                // Add the point to the path points
                Vector3 point = Vector3.Lerp(start, end, t);

                // Project the point onto the NavMesh surface
                slicedPath.Add(point);
            }
        }

        // We need to dynamically create a card in order to subscribe it to the stack
        Card moveCard = new Card();
        moveCard._trigger += () =>
        {
            _agent.destination = vect;
            StartCoroutine(UpdatePath(slicedPath.ToArray()));
        };
        moveCard._duration = _lastCalculatedWalkTime;

        if (false == GameObject.Find("Player").GetComponent<QueueComponent>().AddToQueue(moveCard))
        {
            Debug.Log("error in movement card generation");
        }
     }
    

    // Coroutine to update the path as the agent moves
     IEnumerator UpdatePath(Vector3[] path)
    {
        // Wait for the agent to reach the destination
        //while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
        while (path.Length > 1)
        {
            if (Vector3.Magnitude(path[path.Length-1] - path[0]) > Vector3.Magnitude(path[path.Length-1] - GameObject.Find("Player").transform.position))
            {
                Vector3[] newPath = new Vector3[path.Length - 1];
                for (int i = 0; i < path.Length - 1; i++)
                {
                    newPath[i] = path[i + 1];
                }
                path = newPath;
                _paths[0] = path;
                TrailCalculator.DrawPath(_paths, ref _lineRenderer);
            }
            yield return null;
        }
        _paths.RemoveAt(0);
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
     public void ClearPath()
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