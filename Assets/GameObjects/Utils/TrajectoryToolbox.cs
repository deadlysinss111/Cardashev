using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;
using static UnityEngine.UI.Image;
using System.IO;

public static class TrajectoryToolbox
{
    /*
      ----------------------------------------
       DrawPath overloads below.
       This method draws a LOT of small segments representing either :
          - The preview of a move
          - The path the player is taking
      ----------------------------------------
    */

    static public void DrawPath(List<List<Vector3>> paths, ref LineRenderer lineRenderer)
    {
        List<Vector3> pathPoints = new List<Vector3>();
        foreach (List<Vector3> path in paths)
        {
            List<Vector3> listToPoints = DrawPathBody(path.ToArray());
            foreach (Vector3 point in listToPoints)
            {
                pathPoints.Add(point);
            }
        }

        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());// Update the line renderer positions
    }

    static public void DrawPath(List<Vector3[]> paths, ref LineRenderer lineRenderer)
    {
        List<Vector3> pathPoints = new List<Vector3>();
        foreach (Vector3[] path in paths)
        {
            List<Vector3> listToPoints = DrawPathBody(path);
            foreach (Vector3 point in listToPoints)
            {
                pathPoints.Add(point);
            }
        }

        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());// Update the line renderer positions
    }
    static public void DrawPath(List<NavMeshPath> paths, ref LineRenderer lineRenderer)
    {
        List<Vector3> pathPoints = new List<Vector3>();
        foreach (NavMeshPath path in paths)
        {
            List<Vector3> listToPoints = DrawPathBody(path.corners);
            foreach(Vector3 point in listToPoints)
            {
                pathPoints.Add(point);
            }
        }

        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());// Update the line renderer positions
    }

    static public void DrawPath(NavMeshPath path, ref LineRenderer lineRenderer)
    {
        List<Vector3> pathPoints = DrawPathBody(path.corners);

        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());// Update the line renderer positions
    }

    static public void DrawPath(List<Vector3> path, ref LineRenderer lineRenderer)
    {
        List<Vector3> pathPoints = DrawPathBody(path.ToArray());

        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());// Update the line renderer positions
    }

    static public void DrawPath(Vector3[] path, ref LineRenderer lineRenderer)
    {
        List<Vector3> pathPoints = DrawPathBody(path);

        // Set positions for the line renderer
        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());// Update the line renderer positions
    }

    static private List<Vector3> DrawPathBody(Vector3[] path)
    {
        List<Vector3> pathPoints = new List<Vector3>();

        // Iterate through each segment between corners
        for (int i = 0; i < path.Length - 1; i++)
        {
            // Get the start and end points of the segment
            Vector3 start = path[i];
            Vector3 end = path[i + 1];

            // Interpolate points along the segment between start and end
            int segments = Mathf.CeilToInt(Vector3.Distance(start, end) / 0.1f); // Adjust segment length as needed
            for (int j = 0; j <= segments; j++)
            {
                // Calculate the point along the segment
                float t = (float)j / segments;

                // Add the point to the path points
                Vector3 point = Vector3.Lerp(start, end, t);

                // Project the point onto the NavMesh surface
                if(false == float.IsNaN(point.x))
                    pathPoints.Add(ProjectToNavMeshSurface(point));
            }
        }

        return pathPoints;
    }

    // \_ ↑ DrawPath overloads above ↑ _/


    // Project a point onto the NavMesh surface. This is called for every point of the preview, and so gets called a lot
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


    /*
      ----------------------------------------
       BellCurve overloads below.
       This method either :
          - Returns the points of the curve and draws the curve
          - Returns the points of the curve
          - Draw the curve only
      ----------------------------------------
    */

    static public void BellCurve(Vector3 origin, Vector3 velocity, ref LineRenderer lineRenderer, out List<Vector3> pathPoints)
    {
        // Calling the logic body
        pathPoints = BellCurveBody(origin, velocity);

        // Set positions for the line renderer
        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());// Update the line renderer positions
    }

    static public void BellCurve(Vector3 origin, Vector3 velocity, out List<Vector3> pathPoints)
    {
        // Calling the logic body
        pathPoints = BellCurveBody(origin, velocity);
    }

    static public void BellCurve(Vector3 origin, Vector3 velocity, ref LineRenderer lineRenderer)
    {
        // Calling the logic body
        List<Vector3> pathPoints = BellCurveBody(origin, velocity);
        
        // Set positions for the line renderer
        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());// Update the line renderer positions
    }

    static private List<Vector3> BellCurveBody(Vector3 origin, Vector3 velocity)
    {
        List<Vector3> pathPoints = new List<Vector3>();

        // We initialize base values
        float step = 0.01f;
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

        return pathPoints;
    }


    static public Vector3 BellCurveInitialVelocity(Vector3 startPoint, Vector3 endPoint, float apex)
    {
        float gravity = Physics.gravity.magnitude;

        // Calculate distance and direction
        Vector3 direction = endPoint - startPoint;
        float horizontalDistance = new Vector3(direction.x, 0, direction.z).magnitude;

        // Calculate the initial vertical velocity to reach the desired max height
        float vy0 = Mathf.Sqrt(2 * gravity * apex);

        // Calculate the total time of flight (up and down)
        float timeToApex = vy0 / gravity;
        float totalTime = 2 * timeToApex;

        // Calculate the required horizontal velocity
        float vx0 = horizontalDistance / totalTime;

        // Combine horizontal and vertical velocities
        Vector3 initialVelocity = new Vector3(vx0 * direction.normalized.x, vy0, vx0 * direction.normalized.z);

        return initialVelocity;
    }


    // Raycas twith a bll curve shape
    static public UnityEngine.Transform RaycastBellCurve(Vector3 origin, Vector3 end, float apex)
    {
        return RaycastBellCurve(origin, BellCurveInitialVelocity(origin, end, apex));
    }

    static public UnityEngine.Transform RaycastBellCurve(Vector3 origin, Vector3 velocity)
    {
        // We initialize base values
        float step = 0.01f;
        Vector3 virtualPos = origin;
        Vector3 nextPos;
        float overlap;

        // This loop will calculate next position, check if we hit something and add point to draw in prediction each iteration 
        for (int i = 1; i < 500; i++)
        {
            nextPos = virtualPos + velocity * step;
            velocity += Physics.gravity * step;
            // Overlap our rays by small margin to ensure we never miss a surface
            overlap = Vector3.Distance(virtualPos, nextPos) * 1.1f;

            //When hitting a surface we want to show the surface marker and stop updating our line
            if (Physics.Raycast(virtualPos, velocity.normalized, out RaycastHit hit, overlap))
            {
                return hit.transform;
            }

            virtualPos = nextPos;
        }

        return null;
    }
}
