using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private const string IDLE = "Idle";
    private const string WALK = "Walk";

    private CustomActions input;

    private NavMeshAgent agent;
    private Animator animator;
    private LineRenderer lineRenderer;

    [Header("Movement")]
    [SerializeField] private ParticleSystem clickEffect;

    [SerializeField] private LayerMask clickableLayers;

    private float lookRotationSpeed = 8f;
    private List<Vector3> pathPoints = new List<Vector3>();
    private Coroutine waitForConfirmationCoroutine;

    // Initialization
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();

        input = new CustomActions();
        AssignInputs();
    }

    // Assign input actions
    private void AssignInputs()
    {
        input.Main.Move.performed += ctx => ClickToVisualize();// Handle click to visualize the path
    }

    // Handle click to visualize the path
    private void ClickToVisualize()
    {
        // Raycast to the clicked point
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))// Check if the hit point is on the NavMesh
        {
            // Cancel the previous confirmation waiting coroutine
            if (waitForConfirmationCoroutine != null)
            {
                StopCoroutine(waitForConfirmationCoroutine);
                ClearPath();
            }

            // Calculate the path to the clicked point
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(hit.point, path))
            {
                DrawPath(path);
            }

            // Instantiate click effect at the clicked point
            if (clickEffect != null)
            {
                Instantiate(clickEffect, hit.point + new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
            }

            // Start waiting for confirmation
            waitForConfirmationCoroutine = StartCoroutine(WaitForConfirmation(hit.point));
        }
    }

    // Coroutine to wait for confirmation input
    private IEnumerator WaitForConfirmation(Vector3 destination)
    {
        // Wait for the confirmation input
        while (!input.Main.Confirm.triggered)
        {
            yield return null;
        }

        // Update agent destination only when confirmed
        agent.destination = destination;
        ClearPath();
        StartCoroutine(UpdatePath());
    }

    // Draw the path using line renderer
    private void DrawPath(NavMeshPath path)
    {
        pathPoints.Clear();

        // Add the first point
        pathPoints.Add(transform.position);

        // Iterate through each segment between corners
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            // Get the start and end points of the segment
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i + 1];

            GetWalkTime(end);

            // Interpolate points along the segment between start and end
            int segments = Mathf.CeilToInt(Vector3.Distance(start, end) / 0.1f); // Adjust segment length as needed
            for (int j = 0; j <= segments; j++)
            {
                // Calculate the point along the segment
                float t = (float)j / segments;

                // Add the point to the path points
                Vector3 point = Vector3.Lerp(start, end, t);

                // Project the point onto the NavMesh surface
                pathPoints.Add(ProjectToNavMeshSurface(point));
            }
        }

        // Set positions for the line renderer
        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());// Update the line renderer positions
    }

    // Project a point onto the NavMesh surface
    private Vector3 ProjectToNavMeshSurface(Vector3 point)
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
    private IEnumerator UpdatePath()
    {
        // Wait for the agent to reach the destination
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            // Update the path as the agent moves
            Vector3 playerPosition = transform.position;
            while (pathPoints.Count > 0 && Vector3.Distance(playerPosition, pathPoints[0]) < 0.1f)
            {
                // Remove the first point if the player is close enough
                pathPoints.RemoveAt(0);

                // Set positions for the line renderer
                lineRenderer.positionCount = pathPoints.Count;

                // Update the line renderer positions
                lineRenderer.SetPositions(pathPoints.ToArray());
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
        if (agent.CalculatePath(destination, path))
        {
            Debug.Log("Path time: " + GetPathTime(path));
            return GetPathTime(path);
        }
        return 0;
    }

    // Get the time to traverse the path
    private float GetPathTime(NavMeshPath path)
    {
        // Calculate the time to traverse the path
        float time = 0;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            // Get the start and end points of the segment
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i + 1];

            // Add the time to traverse the segment
            time += Vector3.Distance(start, end) / agent.speed;
        }
        return time;
    }

    // Clear the path from the line renderer
    private void ClearPath()
    {
        lineRenderer.positionCount = 0;
    }

    // Enable input actions
    private void OnEnable()
    {
        input.Enable();
    }

    // Disable input actions
    private void OnDisable()
    {
        input.Disable();
    }

    // Update is called once per frame
    private void Update()
    {
        FaceTarget();
        SetAnimations();
    }

    // Rotate the player to face the target destination
    private void FaceTarget()
    {
        // Calculate the direction to the target destination
        Vector3 direction = (agent.destination - transform.position).normalized;

        // Rotate the player towards the target destination
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Smoothly rotate the player
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    // Set animations based on agent velocity
    private void SetAnimations()
    {
        if (agent.velocity == Vector3.zero)
        {
            animator.Play(IDLE);
        }
        else
        {
            animator.Play(WALK);
        }
    }
}