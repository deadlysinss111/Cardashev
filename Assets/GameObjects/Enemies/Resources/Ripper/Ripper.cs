using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ripper : Enemy
{
    override protected void Act()
    {
        _isMoving = false;
    }

    override protected void Move()
    {
        _isMoving = true;
    }

    // Jump up to 8 tiles in target's direction
    private IEnumerator Jump()
    {
        _timeBeforeDecision = 3;

        Vector3 velocity = TrajectoryToolbox.BellCurveInitialVelocity(transform.position, _target.transform.position, 7);

        LookInDirectionTarget(velocity, 8f);

        // We need to disable the agent in order to be able to manipulate rigidbody's velocity
        _agent.enabled = false;

        // ANIM HERE
        while (_timeBeforeDecision > 3)
        {
            yield return null;
        }
        // ANIM HERE

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = velocity;

        // Calculate the curve for the jump
        List<Vector3> curve;
        TrajectoryToolbox.BellCurve(transform.position, Vector3.ClampMagnitude(_target.transform.position, 8), out curve);


        // Calculate the time to traverse the path
        float time = 0;
        float speed = Vector3.Magnitude(velocity);
        for (int i = 0; i < curve.Count - 1; i++)
        {
            // Get the start and end points of the segment
            Vector3 start = curve[i];
            Vector3 end = curve[i + 1];

            // Add the time to traverse the segment
            time += Vector3.Distance(start, end) / speed;
        }

        _timeBeforeDecision += time;

        StartCoroutine(SetBackAgent());
    }

    IEnumerator SetBackAgent()
    {
        while (_timeBeforeDecision > 0.1f)
        {
            yield return null;
        }
        _agent.enabled = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
