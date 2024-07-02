using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class Murlock : Enemy
{
    [SerializeField] GameObject _swipeHitbox;
    int _dmg;

    private new void Start()
    {
        base.Start();
        _name = "Murlock";
        _agent.speed = 0.5f;
        _dmg = 15;
    }

    // Enemy's decision
    override protected void Act()
    {
        _isMoving = false;

        float dist = Vector3.Magnitude(_target.transform.position - transform.position);
        if(dist < 4)
        {
            StartCoroutine(Swipe());
        }
        else if(dist > 10)
        {
            StartCoroutine(Jump());
        }
        else if(gameObject.GetComponent<StatManager>()._armor == 0)
        {
            if(Random.Range(0, 2) == 0)
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
}
