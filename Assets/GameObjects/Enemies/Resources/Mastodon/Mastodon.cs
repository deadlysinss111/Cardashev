using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Mastodon : Enemy
{
    [SerializeField] GameObject _swipeHitbox;
    int _dmg;
    GameObject[] _barrels;

    private new void Start()
    {
        base.Start();
        _name = "Murlock";
        _agent.speed = 5.5f;
        _dmg = 15;
        
    }

    // Enemy's decision
    override protected void Act()
    {
        _isMoving = false;

        _barrels = GameObject.FindGameObjectsWithTag("Spillable");

        GameObject closestBarrel = FindClosestBarrel(out float distToBarrel);
        if(distToBarrel <= 20)
        {
            Barrel barrelScript = HierarchySearcher.FindChildRecursively(closestBarrel.transform, "Body").GetComponent<Barrel>();
            if (barrelScript._targetedBy == null || barrelScript._targetedBy == gameObject)
            {
                if (Vector3.Magnitude(closestBarrel.transform.position - transform.position) <= 4)
                    SpillBarrel(closestBarrel);
                else
                {
                    barrelScript._targetedBy = gameObject;
                    MoveToClosestBarrel();
                }
                return;
            }
        }

        float dist = Vector3.Magnitude(_target.transform.position - transform.position);
        if (dist < 4)
        {
            StartCoroutine(Swipe());
            return;
        }
        else if (dist > 10)
        {
            if (CheckForJump())
                return;
        }
        if (gameObject.GetComponent<StatManager>()._armor == 0)
        {
            if (Random.Range(0, 2) == 0)
            {
                StartCoroutine(ArmoreUp());
            }
            else
            {
                Move();
            }
        }
        else
        {
            Move();
        }
    }

    // Melee attack
    private IEnumerator Swipe()
    {
        _timeBeforeDecision = 3;
        // ANIM HERE
        Vector3 dir = Vector3.Normalize(new Vector3(_target.transform.position.x - transform.position.x, 0, _target.transform.position.z - transform.position.z));
        LookInDirectionTarget(dir, 8f);
        while (_timeBeforeDecision > 1)
        {
            yield return null;
        }
        Instantiate(_swipeHitbox, transform.position + dir * _swipeHitbox.transform.localScale.z * 0.75f, Quaternion.LookRotation(dir)).GetComponent<AOEVisual>()._dmg = _dmg;
    }

    // Gain armor
    private IEnumerator ArmoreUp()
    {
        print("armoring up");
        _timeBeforeDecision = 3;
        // ANIM HERE
        while (_timeBeforeDecision > 1)
        {
            yield return null;
        }
        gameObject.GetComponent<StatManager>()._armor += Random.Range(14, 17);
    }

    bool CheckForJump()
    {
        // Check for the tile of jump end to know if the enemy can jump to it
        Vector3 endPos = _target.transform.position - Vector3.Normalize(_target.transform.position - transform.position) * 5;
        Physics.Raycast(endPos + new Vector3(0, 3, 0), Vector3.down, out RaycastHit hit);
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
        while(_timeBeforeDecision > 0.1f)
        {
            yield return null;
        }
        _agent.enabled = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    protected override void Move()
    {
        _isMoving = true;

        Vector3 dest;
        dest = _target.transform.position;
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
        while( _timeBeforeDecision > time)
        {
            yield return null;
        }

        Vector3 dest = _target.transform.position;
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _agent.SetDestination(dest);
        LookInDirectionTarget(dest, 8f);
    }

    public void AcideBuff()
    {
        _dmg = 25;
        StartCoroutine(HealSelf(Random.Range(5, 8)));
        StartCoroutine(SetBackDmg(Random.Range(5, 7)));
    }

    IEnumerator HealSelf(float remainingLoops)
    {
        while(remainingLoops > 0)
        {
            gameObject.GetComponent<StatManager>().Heal(2);
            yield return new WaitForSeconds(1);
            remainingLoops -= 1;
        }
    }

    IEnumerator SetBackDmg(float timeLeft)
    {
        while(timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        _dmg = 15;
    }

    GameObject FindClosestBarrel(out float dist)
    {
        GameObject closestBarrel = null;
        float closestDist = 100;
        foreach(GameObject barrel in _barrels)
        {
            dist = Vector3.Magnitude(barrel.transform.position - transform.position);
            if( dist < closestDist)
            {
                closestBarrel = barrel;
                closestDist = dist;
            }
        }
        dist = closestDist;
        return closestBarrel;
    }

    void MoveToClosestBarrel()
    {
        _isMoving = true;

        _target = FindClosestBarrel(out float dist);
        Vector3 dest;
        dest = _target.transform.position;
        dest -= 2* Vector3.Normalize(dest - transform.position);
        dest += new Vector3(0, 2, 0);

        Physics.Raycast(dest, Vector3.down, out RaycastHit hit);
        dest = hit.transform.position + new Vector3(0.5f, 0.5f, 0.5f);

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _agent.SetDestination(dest);
        LookInDirectionTarget(dest, 8f);

        _timeBeforeDecision = GetPathTime(path);
    }

    void SpillBarrel(GameObject barrel)
    {
        GameObject body = HierarchySearcher.FindChildRecursively(barrel.transform, "Body");
        body.GetComponent<Barrel>().Spill(barrel.transform.position - transform.position);
        _target = GI._PlayerFetcher();
        barrel.tag = "Untagged";
        _timeBeforeDecision = 2;
    }
}