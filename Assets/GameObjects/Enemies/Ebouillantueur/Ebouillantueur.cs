using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Ebouillantueur : Enemy
{
    [SerializeField] GameObject _bullet;

    private new void Start()
    {
        base.Start();
        _name = "Ebouillantueur";
        _agent.speed = 3f;
    }


    // Enemy's decision
    override protected void Act()
    {
        _isMoving = false;

        if(UnityEngine.Random.Range(0, 3)  == 0 )
        {
            Move();
        }
        else
        {
            StartCoroutine(Shoot());
        }
    }


    IEnumerator Shoot()
    {
        if(UnityEngine.Random.Range(0, 2) == 0 )
        {
            //GameObject target
        }
        _timeBeforeDecision = 2.5f;
        while( _timeBeforeDecision < 1.5f )
        {
            yield return null;
        }

        // Fires a projectile
        GameObject bullet = Instantiate(_bullet);
        bullet.transform.position = transform.position + new Vector3(0, 1, 0);
        Vector3 velocity= TrajectoryToolbox.BellCurveInitialVelocity(transform.position + new Vector3(0, 1, 0), _target.transform.position, 4);
        bullet.GetComponent<Rigidbody>().velocity = velocity;
    }


    protected override void Move()
    {
        _isMoving = true;

        Vector3 dest =  RandomNavmeshLocation(3, transform.position);
        _agent.SetDestination(dest);
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _timeBeforeDecision = GetPathTime(path);
    }
}
