using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicOne : Enemy
{
    [SerializeField] protected int _healthDanger;

    override protected void Act()
    {
        int rdm = Random.Range(0, 2);
        if (rdm == 0)
        {
            _queueTimer = DecidePath();
        }
        else if (rdm == 1)
        {
            _queueTimer = _enemyDeckManager.Play();
        }
    }

    float DecidePath()
    {
        Vector3 towardPlayer = _player.transform.position - transform.position;


        Vector3 dest; 
        if (_enemyHandler.Health < _healthDanger)
        {
            Vector3 awayPlayer = transform.position - towardPlayer;
            dest = RandomNavmeshLocation(4f, awayPlayer);
            dest -= awayPlayer / 2;
        }
        else
        {
            dest = RandomNavmeshLocation(4f, towardPlayer);
            dest -= towardPlayer / 2;
        }
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _agent.SetDestination(dest);
        return GetPathTime(path);
    }
}
