using System.Collections.Generic;
using UnityEngine;

public class DebugRayCaster : MonoBehaviour
{
    public struct DebugRayCast
    {
        public Vector3 position;
        public Vector3 direction;
        public float distance;
        public Color color;
    }

    private static readonly List<DebugRayCast> _rays = new();

    private void Update()
    {
        foreach (var ray in _rays)
        {
            Debug.DrawRay(ray.position, ray.direction, ray.color, ray.distance);
        }
    }

    public static DebugRayCast CreateDebugRayCast(Vector3 position, Vector3 direction)
    {
        return CreateDebugRayCast(position, direction, Mathf.Infinity, Color.white);
    }
    public static DebugRayCast CreateDebugRayCast(Vector3 position, Vector3 direction, float distance)
    {
        return CreateDebugRayCast(position, direction, distance, Color.white);
    }
    public static DebugRayCast CreateDebugRayCast(Vector3 position, Vector3 direction, Color color)
    {
        return CreateDebugRayCast(position, direction, Mathf.Infinity, color);
    }
    public static DebugRayCast CreateDebugRayCast(Vector3 position, Vector3 direction, float distance, Color color)
    {
        DebugRayCast d = new DebugRayCast
        {
            position = position,
            direction = direction,
            distance = distance,
            color = color
        };

        _rays.Add(d);

        return d;
    }
}