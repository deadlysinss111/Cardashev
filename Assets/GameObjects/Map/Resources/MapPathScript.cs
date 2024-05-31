using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class MapPathScript : MonoBehaviour
{
    Spline _spline;
    private int _splineIndex;
    private float _time;

    float3 position;
    float3 tangent;
    float3 upVector;

    private void Awake()
    {
        _spline = GetComponent<SplineContainer>().Spline;
    }

    private void Start()
    {

    }

    public void SampleSplineWidth(float time, out Vector3 p1, out Vector3 p2)
    {
        //_splineContainer.Evaluate(_splineIndex, time, out position, out tangent, out upVector);

        float3 right = Vector3.Cross(tangent, upVector).normalized;
        p1 = position + Vector3.forward * right;
        p2 = position + Vector3.forward * -right;
    }

    public void SetPathPoints(GameObject point1, GameObject point2)
    {
        BezierKnot knot = _spline[0];
        knot.Position = point1.transform.position;
        _spline[0] = knot;

        knot = _spline[1];
        knot.Position = point2.transform.position;
        _spline[1] = knot;

        //SetShapePosition(point1.transform.position, point2.transform.position);
    }

    public void LockPath()
    {

    }

    void SetShapePosition(Vector3 start, Vector3 end)
    {
        Vector3 mid = new Vector3((start.x + end.x) / 2, (start.y + end.y) / 2, (start.z + end.z) / 2);
        transform.position = mid;
    }
}
