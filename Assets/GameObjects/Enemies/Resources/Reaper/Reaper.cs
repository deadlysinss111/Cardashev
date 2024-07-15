using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Reaper : Enemy
{
    int _dmg = 777;

    float startScratchSpeed = -1f;

    DebugRayCaster.DebugRayCast _debugCurrentDest;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _name = "Reaper";
        _agent.speed = 5.5f;
        _dmg = 777;
    }

    override protected void Act()
    {
        _isMoving = false;

        float dist = Vector3.Distance(_target.transform.position, transform.position);
        print($"Reaper distance: {dist}");

        if (dist > 20)
        {
            CheckForJump(); // Too far, get closer to attempt an attacking
            return;
        }    

        if (GetComponent<StatManager>().Health < GetComponent<StatManager>().BaseHealth/4)
        {

        }
        else
        {
            Scratch(); // Attack 1
        }
    }

    protected override void Move()
    {
        _isMoving = true;

        Vector3 dest;
        dest = _target.transform.position + Random.onUnitSphere * 30;
        _debugCurrentDest = DebugRayCaster.CreateDebugRayCast(dest, Vector3.up, Color.green);
        print("Choose move");

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _agent.SetDestination(dest);
        LookInDirectionTarget(dest, 8f);

        _timeBeforeDecision = GetPathTime(path);

        // Once the half of the movement done, we want the enemy to refresh his destination to match target's movments
        // TODO: Decide of a tile to snap too, to ensure we don't stop in the middle of the board
        StartCoroutine(reOrient(_timeBeforeDecision / 2));
    }

    private IEnumerator reOrient(float time)
    {
        while (_timeBeforeDecision > time)
        {
            yield return null;
        }

        Vector3 dest = _target.transform.position;
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _agent.SetDestination(dest);
        LookInDirectionTarget(dest, 8f);
    }

    bool CheckForJump()
    {
        // Check for the tile of jump end to know if the enemy can jump to it
        Vector3 endPos = _target.transform.position - Vector3.Normalize(_target.transform.position - transform.position) * 10;
        Physics.Raycast(endPos + new Vector3(0, 3, 0), Vector3.down);
        Transform endTile = TrajectoryToolbox.RaycastBellCurve(transform.position, endPos, 7); // Need to have a jump apex :)
        if (endTile.gameObject.CompareTag("TMTopology"))
        {
            StartCoroutine(Jump(endPos));
            return true;
        }
        return false;
    }

    // Jump to the position passed as argument
    private IEnumerator Jump(Vector3 endPos)
    {
        _timeBeforeDecision = 3;

        Vector3 velocity = TrajectoryToolbox.BellCurveInitialVelocity(transform.position, endPos, 7);

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

    void Scratch()
    {
        _agent.enabled = false;
        Rigidbody rBody = GetComponent<Rigidbody>();
        rBody.isKinematic = false;

        LookInDirectionTarget(_target.transform.position, 8f);

        Vector3 dir = (transform.position - _target.transform.position).normalized;
        rBody.velocity = dir;

        startScratchSpeed = 10f;
        _isMoving = true;

        StartCoroutine(EaseScratch());
    }

    IEnumerator EaseScratch()
    {
        while (startScratchSpeed > 0)
        {
            EaseOutCubic(5f);
            yield return null;
        }
    }

    float EaseOutCubic(float t) => 1 - Mathf.Pow(1 - t, 3);
}
