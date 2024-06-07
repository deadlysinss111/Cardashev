using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class Murlock : Enemy
{
    [SerializeField] GameObject _swipeZone;
    int _dmg;

    private new void Start()
    {
        base.Start();
        _agent.speed = 1.5f;
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
        _queueTimer = 3;
        // ANIM HERE
        while(_queueTimer > 1)
        {
            yield return null;
        }
        Vector3 dir = Vector3.Normalize(new Vector3(_target.transform.position.x - transform.position.x, 0, _target.transform.position.z - transform.position.z));
        Instantiate(_swipeZone, transform.position + dir * _swipeZone.transform.localScale.z * 0.75f, Quaternion.LookRotation(dir)).GetComponent<AOEVisual>()._dmg = _dmg;
    }

    // Gain armor
    private IEnumerator ArmoreUp()
    {
        print("armoring up");
        _queueTimer = 3;
        // ANIM HERE
        while (_queueTimer > 1)
        {
            yield return null;
        }
        gameObject.GetComponent<StatManager>()._armor += Random.Range(14, 17);
    }

    // Jump up to 8 tiles in target's direction
    private IEnumerator Jump()
    {
        _queueTimer = 3;

        // We need to disable the agent in order to be able to manipulate rigidbody's velocity
        _agent.enabled = false;

        // ANIM HERE
        while (_queueTimer > 3)
        {
            yield return null;
        }
        // ANIM HERE

        Vector3 velocity = TrailCalculator.BellCurveInititialVelocity(transform.position, _target.transform.position, 7);
        gameObject.GetComponent<Rigidbody>().velocity = velocity;

        // Calculate the curve for the jump
        List<Vector3> curve;
        TrailCalculator.BellCurve(transform.position, Vector3.ClampMagnitude(_target.transform.position, 8), out curve);

        
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

        _queueTimer += time;
        
        StartCoroutine(SetBackAgent());
    }

    IEnumerator SetBackAgent()
    {
        while(_queueTimer > 0.1f)
        {
            yield return null;
        }
        _agent.enabled = true;
    }

    protected override void Move()
    {
        _isMoving = true;

        Vector3 dest;
        dest = _target.transform.position;
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _agent.SetDestination(dest);

        _queueTimer = GetPathTime(path);

        // Once the half of the movement done, we want the enemy to refresh his destination to match target's movments
        StartCoroutine(reOrient(_queueTimer / 2));
    }

    private IEnumerator reOrient(float time)
    {
        while( _queueTimer > time)
        {
            yield return null;
        }

        Vector3 dest = _target.transform.position;
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _agent.SetDestination(dest);
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
