using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.AI;

public static class TrailCalculator
{

    // Draw the path using line renderer
    static public void DrawPath(NavMeshPath path, ref LineRenderer lineRenderer)
    {
        List<Vector3> pathPoints = new List<Vector3>();

        // Add the first point
        //pathPoints.Add(transform.position);

        // Iterate through each segment between corners
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            // Get the start and end points of the segment
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i + 1];

            //_lastCalculatedWalkTime = GetWalkTime(end);

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
    static public Vector3 ProjectToNavMeshSurface(Vector3 point)
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

    static public void BellCurve(Vector3 origin, Vector3 velocity, ref LineRenderer lineRenderer, out List<Vector3> pathPoints)
    {
        pathPoints = new List<Vector3>();

        // We initialize base values
        float step = 0.01f;
        //Vector3 virtualPos = GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos;
        Vector3 virtualPos = origin;
        Vector3 nextPos;
        float overlap;

        // This loop will calculate next position, check if we hit something and add point to draw in prediction each iteration 
        for (int i = 1; i < 500; i++)
        {
            nextPos = virtualPos + velocity * step;
            velocity += Physics.gravity * step;
            pathPoints.Add(virtualPos);

            // Overlap our rays by small margin to ensure we never miss a surface
            overlap = Vector3.Distance(virtualPos, nextPos) * 1.1f;

            //When hitting a surface we want to show the surface marker and stop updating our line
            if (Physics.Raycast(virtualPos, velocity.normalized, out RaycastHit hit, overlap))
            {
                break;
            }

            virtualPos = nextPos;
        }
        // Set positions for the line renderer
        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());// Update the line renderer positions
    }

    static public Vector3 BellCurveInititialVelocity(Vector3 startPoint, Vector3 endPoint, float apex)
    {
        float maxHeight = apex;
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

        return initialVelocity;
    }

}
