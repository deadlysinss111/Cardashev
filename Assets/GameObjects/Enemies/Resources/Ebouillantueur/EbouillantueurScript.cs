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
    int _movePity;
    float _meleeCooldown;
    PlayerManager _player;
    GameObject ACID;

    private new void Start()
    {
        base.Start();
        _name = "Ebouillantueur";
        _agent.speed = 3f;
        _movePity = 0;
        _player = GI._PManFetcher();
        ACID = (GameObject)Resources.Load("Debug Zone/Interactibles/Prefabs/Acid");
    }


    // Enemy's decision
    override protected void Act()
    {
        _isMoving = true; // DO NOT REMOVE THIS LINE
        _meleeCooldown -= Time.deltaTime;

        int perc = UnityEngine.Random.Range(1, 101);

        if (perc >= _movePity + (PlayerCloserThan(20) ? 60 : 0))
        {
            _movePity += 15;
            perc = UnityEngine.Random.Range(1, 101);

            GameObject closestMonster = GameObject.Find("Mastodon(Clone)");

            if (PlayerCloserThan(5) && _meleeCooldown <= 0)
            {
                print("Meleed player");
                _meleeCooldown = 10;
                MeleeAttack();
            }
            else if (perc >= 20 || closestMonster == null)
            {
                print("Shot at player");
                StartCoroutine(ShootPlayer());
            }
            else
            {
                print("Supported");
                StartCoroutine(SupportMonster(closestMonster));
            }
        }
        else
        {
            print("Moved FDP");
            _movePity = 0;
            Move();
        }
    }

    bool PlayerCloserThan(float distance)
    {
        return Vector3.Distance(_player.transform.position, transform.position) / 2 < distance;
    }

    void MeleeAttack()
    {
        Collider[] temp = Physics.OverlapSphere(transform.position - Vector3.up * 2, 0.1f);
        print(temp[0]);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Instantiate(ACID, temp[0].transform.position + new Vector3(i * 2, 1.1f, j * 2), Quaternion.identity);
            }
        }
    }

    IEnumerator ShootPlayer()
    {
        /*if(UnityEngine.Random.Range(0, 2) == 0 )
        {
            //GameObject target
        }*/
        _timeBeforeDecision = 2f;
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

    IEnumerator SupportMonster(GameObject target)
    {
        /*if(UnityEngine.Random.Range(0, 2) == 0 )
        {
            //GameObject target
        }*/
        _timeBeforeDecision = 2f;
        while (_timeBeforeDecision < 1.5f)
        {
            yield return null;
        }

        // Fires a projectile
        GameObject bullet = Instantiate(_bullet);
        bullet.transform.position = transform.position + new Vector3(0, 1, 0);
        Vector3 velocity = TrajectoryToolbox.BellCurveInitialVelocity(transform.position + new Vector3(0, 1, 0), target.transform.position, 2);
        bullet.GetComponent<Rigidbody>().velocity = velocity;
    }

    protected override void Move()
    {
        if (false == _agent.isActiveAndEnabled || false == _agent.isOnNavMesh) return;

        _isMoving = true;

        Vector3 dest =  RandomNavmeshLocation(1, transform.position);
        _agent.SetDestination(dest);
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _timeBeforeDecision = GetPathTime(path);
    }
}
