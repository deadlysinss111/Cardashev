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
    float _meleeCooldown;
    bool _fleeing;
    PlayerManager _player;
    GameObject ACID;

    private new void Start()
    {
        base.Start();
        _name = "Ebouillantueur";
        _agent.speed = 3f;
        _player = GI._PManFetcher();
        ACID = (GameObject)Resources.Load("Radioactive Zone/Interactibles/Prefabs/Acid");
    }


    // Enemy's decision
    override protected void Act()
    {
        _isMoving = false; // DO NOT REMOVE THIS LINE
        _meleeCooldown -= Time.deltaTime;

        float playerDist = DistanceToPlayer();

        if (playerDist > 5 && playerDist < 20)
        {
            int perc = UnityEngine.Random.Range(1, 101);

            //GameObject closestMonster = GameObject.Find("Mastodon(Clone)");

            //else // if (perc >= 20 || closestMonster == null)
            {
                //print("Shot at player");
                StartCoroutine(ShootPlayer());
            }
            /*else
            {
                print("Supported");
                StartCoroutine(SupportMonster(closestMonster));
            }*/
        }
        else
        {
            if (playerDist < 5 && _meleeCooldown <= 0)
            {
                //print("Meleed player");
                _meleeCooldown = 10;
                StartCoroutine(MeleeAttack());
            }
            else if (playerDist > 20)
            {
                _fleeing = false;
                Move();
            }
            else
            {
                _fleeing = true;
                Move();
            }
        }
    }

    float DistanceToPlayer()
    {
        return Vector3.Distance(_player.transform.position, transform.position) / 2;
    }

    IEnumerator MeleeAttack()
    {
        Collider[] temp = Physics.OverlapSphere(transform.position - Vector3.up * 2, 0.1f);
        _timeBeforeDecision = 9 * .2f + .5f;
        LayerMask mask = LayerMask.NameToLayer("TMTopology");
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                // TODO : FIX FLYING ACID PLACED ON EMPTY SPACE
                if (Physics.OverlapSphere(temp[0].transform.position + new Vector3(i * 2 - 2, 1.1f, j * 2 - 2), .1f, mask).Length == 0) continue;
                Instantiate(ACID, temp[0].transform.position + new Vector3(i * 2 - 2, 1.1f, j * 2 - 2), Quaternion.identity);
                yield return new WaitForSeconds(.2f);
            }
        }
    }

    IEnumerator ShootPlayer()
    {
        /*if(UnityEngine.Random.Range(0, 2) == 0 )
        {
            //GameObject target
        }*/
        _timeBeforeDecision = 4f;
        while( _timeBeforeDecision < 1.5f )
        {
            yield return null;
        }

        // Fires a projectile
        GameObject bullet = Instantiate(_bullet);
        bullet.transform.position = transform.position + new Vector3(0, 1, 0);
        Vector3 velocity= TrajectoryToolbox.BellCurveInitialVelocity(transform.position + new Vector3(0, 1, 0), _target.transform.position, 2);
        bullet.GetComponent<Rigidbody>().velocity = velocity;
    }

    IEnumerator SupportMonster(GameObject target)
    {
        /*if(UnityEngine.Random.Range(0, 2) == 0 )
        {
            //GameObject target
        }*/
        _timeBeforeDecision = 4f;
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
        Vector3 targetArea;

        // TODO: one of those two logics
        if (_fleeing)
        {
            targetArea = transform.position + Vector3.Normalize(transform.position - _player.transform.position) * 8;
        }
        else
        {
            targetArea = transform.position + Vector3.Normalize(_player.transform.position - transform.position) * 8;
        }


        Vector3 dest =  RandomNavmeshLocation(4, targetArea);

        if (dest == Vector3.zero) throw new Exception("error in targeting AI for ebouillantueur");

        NavMeshPath path = new NavMeshPath();

        if( false == Physics.Raycast(dest + new Vector3(0, 2, 0), Vector3.down, out RaycastHit hit))
            throw new Exception("error in targeting raycast for ebouillantueur");

        dest = hit.transform.position + new Vector3(0, 1, 0);
        _agent.SetDestination(dest);

        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _timeBeforeDecision = GetPathTime(path);
    }
}
