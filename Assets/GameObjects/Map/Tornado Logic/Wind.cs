using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Wind : MonoBehaviour
{
    [SerializeField] Transform _particleOrigin;
    [SerializeField] Transform _anchor;

    public void SetToDirection(Vector3 direction)
    {
        // Movement transformations
        float dist = Vector3.Magnitude(_particleOrigin.position - _anchor.position);
        _particleOrigin.position = _anchor.position - Vector3.Normalize(direction) * dist;

        // Rotation transformations
        _particleOrigin.LookAt(_anchor.position);
    }

}
