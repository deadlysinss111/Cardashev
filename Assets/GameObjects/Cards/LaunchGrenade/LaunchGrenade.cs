using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LaunchGrenade : Card
{
    CustomActions _input;
    [SerializeField] LayerMask _clickableLayers;
    List<Vector3> _pathPoints;

    LineRenderer _lineRenderer;

    private void Awake()
    {
        _input = new CustomActions();
        _lineRenderer = GetComponent<LineRenderer>();
        _pathPoints = new List<Vector3>();
    }

    public override void Effect()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().SetToMovementState();
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

            // In the following snipet, the commented code are those that use not cropped positions
            //if (NavMesh.CalculatePath(_virtualPos, hit.point, NavMesh.AllAreas,  path))
            if (NavMesh.CalculatePath(GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos, alteredPos, NavMesh.AllAreas, path))
            {
                DrawPath(path);
            }

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


//void CalculatePath(Vector3 destination)
//{
//    float step = 0.1f;
//    print("yes");
//    Vector3 virtualPos = GameObject.Find("Player").transform.position;
//    Vector3 virtualVelocity = _grenadeVelocity;
//    while (Abs(virtualPos.x - destination.x) >= 0.1f || Abs(virtualPos.y - destination.y) >= 0.1f || Abs(virtualPos.z - destination.z) >= 0.1f)
//    {
//        virtualPos += virtualVelocity * step;
//        virtualVelocity += Physics.gravity;
//        _pathPoints.Add(virtualPos);
//        print(virtualPos);
//    }
//}

//float Abs(float value)
//{
//    if (value < 0f)
//    {
//        return -value;
//    }
//    return value;
//}