using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EvilLaunchGrenade : LaunchGrenade
{
    CustomActions _input;
    [SerializeField] LayerMask _clickableLayers;
    Coroutine _waitForConfirmationCoroutine;
    List<Vector3> _pathPoints;

    LineRenderer _lineRenderer;

    private void Awake()
    {
        _input = new CustomActions();
        _lineRenderer = GetComponent<LineRenderer>();
        _pathPoints = new List<Vector3>();
        _input.Enable();
    }

    public override void Effect()
    {
        base.Effect();
    }

    protected override void ClickEvent()
    {
        StartCardAction();
        //_input.Main.prev.performed += ctx => Preview();
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
            if (_waitForConfirmationCoroutine != null)
            {
                StopCoroutine(_waitForConfirmationCoroutine);
                ClearPath();
            }

            // Calculate the path to the clicked point
            NavMeshPath path = new NavMeshPath();

            // In the following snipet, the commented code are those that use not cropped positions
            //if (NavMesh.CalculatePath(_virtualPos, hit.point, NavMesh.AllAreas,  path))
            if (NavMesh.CalculatePath(GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos, alteredPos, NavMesh.AllAreas, path))
            {
                DrawPath(path);
            }

            // Start waiting for confirmation
            _waitForConfirmationCoroutine = StartCoroutine(WaitForConfirmation(hit.point));
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

        Object grenade = Resources.Load("Grenade");
        Instantiate(grenade);

        // Trigger the card play event
        base.ClickEvent();
    }

    void ClearPath()
    {
        _lineRenderer.positionCount = 0;
    }

    void DrawPath(NavMeshPath path)
    {
        print("previex");
        _pathPoints.Clear();

        // Add the first point
        _pathPoints.Add(transform.position);

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
}
