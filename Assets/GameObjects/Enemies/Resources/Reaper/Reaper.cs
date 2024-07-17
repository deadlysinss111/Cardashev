using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Reaper : Enemy
{
    public int _dmg = 777;

    float startScratchSpeed = -1f;
    Vector3 baseDirSpeed = Vector3.zero;

    int corId = 0;

    DebugRayCaster.DebugRayCast _debugCurrentDest;

    public UnityEvent _changeOfStateScratch;

    RaycastHit[] _runHits;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _name = "Reaper";
        _agent.speed = 5.5f;
        _dmg = 8;

        _runHits = new RaycastHit[255];

        _changeOfStateScratch = new UnityEvent();
    }

    override protected void Act()
    {
        _isMoving = false;

        float dist = Vector3.Distance(_target.transform.position, transform.position);
        //print($"Reaper distance: {dist}");

        if (dist > 20)
        {
            CheckForJump(); // Too far, get closer to attempt an attacking
            return;
        }    

        if (GetComponent<StatManager>().Health <= GetComponent<StatManager>().BaseHealth/4 && dist <= 5)
        {
            //BitchSlap();
            Scratch(true);
            return;
        }
        else
        {
            Scratch(false); // Attack 1
        }
    }

    protected override void Move()
    {
        _isMoving = true;

        Vector3 dest;
        dest = _target.transform.position + Random.onUnitSphere * 30;
        _debugCurrentDest = DebugRayCaster.CreateDebugRayCast(dest, Vector3.up, Color.green);

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _agent.SetDestination(dest);
        LookInDirectionTarget(dest, 8f);

        _timeBeforeDecision = GetPathTime(path);

        // Once the half of the movement done, we want the enemy to refresh his destination to match target's movments
        // TODO: Decide of a tile to snap too, to ensure we don't stop in the middle of the board
        StartCoroutine(reOrient(_timeBeforeDecision / 2));
    }

    protected void Move(Vector3 dest)
    {
        _isMoving = true;

        _debugCurrentDest = DebugRayCaster.CreateDebugRayCast(dest, Vector3.up, Color.green);

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _agent.SetDestination(dest);
        LookInDirectionTarget(dest, 8f);

        _timeBeforeDecision = GetPathTime(path);

        // Once the half of the movement done, we want the enemy to refresh his destination to match target's movments
        // TODO: Decide of a tile to snap too, to ensure we don't stop in the middle of the board
        StartCoroutine(reOrient(_timeBeforeDecision / 2));
        print("Move End - " + _timeBeforeDecision);
    }

    protected new void CheckPlayerDistance() { }

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

    void Scratch(bool runAwayAfter)
    {
        _agent.enabled = false;
        Rigidbody rBody = GetComponent<Rigidbody>();
        rBody.isKinematic = false;

        LookInDirectionTarget(_target.transform.position, 8f);

        Vector3 dir = (_target.transform.position - transform.position).normalized;
        dir.y = rBody.velocity.y;

        startScratchSpeed = 10f;
        rBody.velocity = dir*startScratchSpeed;
        baseDirSpeed = rBody.velocity;

        _isMoving = true;

        _timeBeforeDecision = Mathf.Infinity;

        corId += 1;
        StartCoroutine(EaseScratch(rBody, corId, runAwayAfter));
    }

    IEnumerator EaseScratch(Rigidbody rBody, int id, bool run)
    {
        //print($"{id} - Start");
        _changeOfStateScratch.Invoke();
        while (rBody.velocity.magnitude > 0.01f)
        {
            yield return null;
        }
        //print($"{id} - Done");
        rBody.velocity = Vector3.zero;
        baseDirSpeed = Vector3.zero;
        startScratchSpeed = -1f;

        _isMoving = false;
        rBody.isKinematic = true;
        _agent.enabled = true;
        _timeBeforeDecision = 3f;
        corId -= 1;
        _changeOfStateScratch.Invoke();

        if (run)
        {
            //print($"{id} - RUN");
            float dist = 10f;

            int hitLen = Physics.SphereCastNonAlloc(transform.position, dist, Vector3.back, _runHits, dist, LayerMask.NameToLayer("TMTopology"));
            if (hitLen == 0)
            {
                Debug.LogWarning("[Reaper] No cases to run away to were found!");
                yield break;
            }
            Transform farestTrans = _runHits[0].transform;
            float currDist = Vector3.Distance(transform.position, farestTrans.position);

            for (int i = 0; i < hitLen; i++)
            {
                RaycastHit hit = _runHits[i];
                float newDist = Vector3.Distance(transform.position, hit.transform.position);
                if (newDist > currDist)
                {
                    currDist = newDist;
                    farestTrans = hit.transform;
                }
            }

            /*while (Vector3.Equals(endPos, Vector3.negativeInfinity))
            {
                // Set up a hard limit like NASA engineers :D
                limit++;
                if (limit > 100)
                {
                    Debug.LogException(new System.StackOverflowException("[Reaper] while loop went over the set limit! Stoping it by force!"));
                    endPos = transform.position + new Vector3(5, 0, 5);
                    break;
                }
                if (Physics.Raycast(targetPos, Vector3.down, out RaycastHit hit, dist * 2) == false) continue;

                if (hit.collider.gameObject.CompareTag("TMTopology"))
                {
                    endPos = hit.collider.gameObject.transform.position;
                    break;
                }
                dist += 5;
            }*/

            Move(farestTrans.position);
        }
    }

    float EaseOutCubic(float t) => 1 - Mathf.Pow(1 - t, 3);

    void BitchSlap()
    {
        List<Collider> colliders = GetComponentInChildren<ReaperAOE>()._colliders;
        // If the player is already in the collider, let's not attack them
        bool skipAttack = false;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                skipAttack = true;
            }
        }
    }
}
