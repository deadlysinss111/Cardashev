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
    static ANIMSTATES AS;

     CustomActions _input;

     NavMeshAgent _agent;
     Animator _animator;
     LineRenderer _lineRenderer;

    [Header("Movement")]
    [SerializeField]  ParticleSystem _clickEffect;

    [SerializeField]  LayerMask _clickableLayers;

     float _lookRotationSpeed = 8f;
     List<Vector3> _pathPoints = new List<Vector3>();
     Coroutine _waitForConfirmationCoroutine;

    float _lastCalculatedWalkTime;

    // Initialization
     void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _lineRenderer = GetComponent<LineRenderer>();

        _input = new CustomActions();
        AssignInputs();
    }

    // Assign input actions
     void AssignInputs()
    {
        _input.Main.Move.performed += ctx => ClickToVisualize();// Handle click to visualize the path
    }

    // Handle click to visualize the path
     void ClickToVisualize()
    {
        // Raycast to the clicked point
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, _clickableLayers))// Check if the hit point is on the NavMesh
        {
            // Cancel the previous confirmation waiting coroutine
            if (_waitForConfirmationCoroutine != null)
            {
                StopCoroutine(_waitForConfirmationCoroutine);
                ClearPath();
            }

            // Calculate the path to the clicked point
            NavMeshPath path = new NavMeshPath();
            if (_agent.CalculatePath(hit.point, path))
            {
                DrawPath(path);
            }

            // Instantiate click effect at the clicked point
            if (_clickEffect != null)
            {
                Instantiate(_clickEffect, hit.point + new Vector3(0, 0.1f, 0), _clickEffect.transform.rotation);
            }

            // Start waiting for confirmation
            _waitForConfirmationCoroutine = StartCoroutine(WaitForConfirmation(hit.point));
        }
    }

    // Coroutine to wait for confirmation input
     IEnumerator WaitForConfirmation(Vector3 destination)
    {
        // Wait for the confirmation input
        while (_input.Main.Confirm.triggered == false)
        {
            yield return null;
        }

        // Update agent destination only when confirmed
        
        ClearPath();

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

    // Draw the path using line renderer
     void DrawPath(NavMeshPath path)
    {
        _pathPoints.Clear();

        // Add the first point
        _pathPoints.Add(transform.position);

        // Iterate through each segment between corners
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            // Get the start and end points of the segment
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i + 1];

            _lastCalculatedWalkTime = GetWalkTime(end);

            // Interpolate points along the segment between start and end
            int segments = Mathf.CeilToInt(Vector3.Distance(start, end) / 0.1f); // Adjust segment length as needed
            for (int j = 0; j <= segments; j++)
            {
                // Calculate the point along the segment
                float t = (float)j / segments;

                // Add the point to the path points
                Vector3 point = Vector3.Lerp(start, end, t);

                // Project the point onto the NavMesh surface
                _pathPoints.Add(ProjectToNavMeshSurface(point));
            }
        }

        // Set positions for the line renderer
        _lineRenderer.positionCount = _pathPoints.Count;
        _lineRenderer.SetPositions(_pathPoints.ToArray());// Update the line renderer positions
    }

    // Project a point onto the NavMesh surface
     Vector3 ProjectToNavMeshSurface(Vector3 point)
    {
        // Project the point onto the NavMesh surface
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point, out hit, 10f, NavMesh.AllAreas))
        {
            // Return the projected point
            return hit.position;
        }
        else
        {
            // Return the original point if projection fails
            Debug.LogWarning("Failed to project point onto NavMesh surface.");
            return point;
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
    public float GetWalkTime(Vector3 destination)
    {
        // Calculate the path to the destination
        NavMeshPath path = new NavMeshPath();
        if (_agent.CalculatePath(destination, path))
        {
            Debug.Log("Path time: " + GetPathTime(path));
            return GetPathTime(path);
        }
        return 0;
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