using System.Collections;
using System.Collections.Generic;
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

    protected override void ClickEvent()
    {
        GameObject.Find("Player").GetComponent<PlayerManager>().SetLeftClickTo(() => Preview());
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

            CalculatePath(alteredPos);

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

    void DrawPath(NavMeshPath path)
    {
        _pathPoints.Clear();

        // Add the first point
        //_pathPoints.Add(transform.position);

        // Iterate through each segment between corners
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            // Get the start and end points of the segment
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i + 1];

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
    void CalculatePath(Vector3 destination)
    {
        _pathPoints.Clear();

        // We initialize base values
        float step = 0.01f;
        Vector3 virtualPos = GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos;
        Vector3 nextPos;
        CalculateInitialVelocity(virtualPos, destination);
        Vector3 virtualVelocity = _grenadeInitVelocity;
        float overlap;

        // This loop will calculate next position, check if we hit something and add point to draw in prediction each iteration 
        for (int i = 1; i < 500; i++)
        {
            nextPos = virtualPos + virtualVelocity * step;
            virtualVelocity += Physics.gravity * step;
            _pathPoints.Add(virtualPos);

            // Overlap our rays by small margin to ensure we never miss a surface
            overlap = Vector3.Distance(virtualPos, nextPos) * 1.1f;

            //When hitting a surface we want to show the surface marker and stop updating our line
            if (Physics.Raycast(virtualPos, virtualVelocity.normalized, out RaycastHit hit, overlap))
            {
                break;
            }

            virtualPos = nextPos;
        }
        // Set positions for the line renderer
        _lineRenderer.positionCount = _pathPoints.Count;
        _lineRenderer.SetPositions(_pathPoints.ToArray());// Update the line renderer positions
    }

    void CalculateInitialVelocity(Vector3 startPoint, Vector3 endPoint)
    {
        float maxHeight = 10f;
        float gravity = Physics.gravity.magnitude;

        // Calculate distance and direction
        Vector3 direction = endPoint - startPoint;
        float horizontalDistance = new Vector3(direction.x, 0, direction.z).magnitude;

        // Calculate the initial vertical velocity to reach the desired max height
        float vy0 = Mathf.Sqrt(2 * gravity * maxHeight);

        // Calculate the total time of flight (up and down)
        float timeToApex = vy0 / gravity;
        float totalTime = 2 * timeToApex;

        // Calculate the required horizontal velocity
        float vx0 = horizontalDistance / totalTime;

        // Combine horizontal and vertical velocities
        Vector3 initialVelocity = new Vector3(vx0 * direction.normalized.x, vy0, vx0 * direction.normalized.z);

        _grenadeInitVelocity = initialVelocity;
    }
}