using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class Ebouillantueur : Enemy
{
    [SerializeField] GameObject _bullet;

    private new void Start()
    {
        base.Start();
        _agent.speed = 3f;
    }

    // Enemy's decision
    override protected void Act()
    {
        _isMoving = false;

        if(Random.Range(0, 3)  == 0 )
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
        if( Random.Range(0, 2) == 0 )
        {
            //GameObject target
        }
        _queueTimer = 2.5f;
        while( _queueTimer < 1.5 )
        {
            yield return null;
        }
        GameObject bullet = (GameObject)Instantiate(_bullet);
        bullet.transform.position = transform.position + new Vector3(0, 1, 0);
        Vector3 velocity= TrailCalculator.BellCurveInititialVelocity(transform.position + new Vector3(0, 1, 0), _target.transform.position, 4);
        bullet.GetComponent<Rigidbody>().velocity = velocity;
    }


    protected override void Move()
    {
        _isMoving = true;

        Vector3 dest =  RandomNavmeshLocation(3, transform.position);
        _agent.SetDestination(dest);
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _queueTimer = GetPathTime(path);
    }
}