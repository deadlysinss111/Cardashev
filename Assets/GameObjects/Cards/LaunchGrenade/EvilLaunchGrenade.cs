using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EvilLaunchGrenade : LaunchGrenade
{
    CustomActions _input;
    [SerializeField] LayerMask _clickableLayers;
    Coroutine _waitForConfirmationCoroutine;
    List<Vector3> _pathPoints;


    LineRenderer _lineRenderer;

    Vector3 _grenadeInitVelocity;

    private void Awake()
    {
        _input = new CustomActions();
        _lineRenderer = GetComponent<LineRenderer>();
        _pathPoints = new List<Vector3>();
        _input.Enable();

        _duration = 2f;
    }

    public override void Effect(GameObject enemy)
    {
        FireGrenade(enemy);
    }

    protected void FireGrenade(GameObject enemy)
    {
        UnityEngine.Object GRENADE = Resources.Load("Grenade");
        GameObject grenade = (GameObject)Instantiate(GRENADE);
        Vector3 pos = enemy.GetComponent<BasicEnemyHandler>()._virtualPos;
        pos.y += 0.5f;
        grenade.GetComponent<Rigidbody>().transform.position = pos;
        grenade.GetComponent<Rigidbody>().velocity = TrailCalculator.BellCurveInititialVelocity(grenade.GetComponent<Rigidbody>().transform.position, GameObject.Find("Player").transform.position, 5);

        // Trigger the card play event
        //base.ClickEvent();
    }

    protected void StartCardAction(GameObject enemy)
    {
        GameObject grenade = (GameObject)Instantiate(Resources.Load("Grenade"));
        Vector3 pos = enemy.transform.position;
        pos.y = pos.y + 0.5f;
        grenade.transform.position = pos;
        grenade.GetComponent<Rigidbody>().velocity = TrailCalculator.BellCurveInititialVelocity(pos, GameObject.Find("Player").transform.position, Mathf.Clamp(Vector3.Distance(pos, GameObject.Find("Player").transform.position), 1f, 5f));
        BetterDebug.Log(grenade.GetComponent<Rigidbody>().velocity, grenade.transform.position);

        // Trigger the card play event
        //base.ClickEvent();
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
