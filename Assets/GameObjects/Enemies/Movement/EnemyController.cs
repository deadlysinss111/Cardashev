using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Could probably be used later
    GameObject _player;

    BasicEnemyHandler _enemyHandler;
    [SerializeField] int _healthDanger;

    NavMeshAgent _agent;
    EnemyDeckManager _enemyDeckManager;

    float _queueTimer;

    void Start()
    {
        _player = GameObject.Find("Player");
        _enemyHandler = GetComponent<BasicEnemyHandler>();
        _agent = GetComponent<NavMeshAgent>();
        _enemyDeckManager = GetComponent<EnemyDeckManager>();

        _enemyHandler._virtualPos = _agent.transform.position;
    }

    void Update()
    {
        _queueTimer -= Time.deltaTime;
        if(_queueTimer <= 0)
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
    }

    float DecidePath()
    {
        Vector3 towardPlayer = _player.transform.position - transform.position;
        

        Vector3 dest;
        if (_enemyHandler.Health <  _healthDanger)
        {
            Vector3 awayPlayer = transform.position - towardPlayer;
            dest = RandomNavmeshLocation(4f, awayPlayer);
            dest -= awayPlayer/2;
        }
        else
        {
            dest = RandomNavmeshLocation(4f, towardPlayer);
            dest -= towardPlayer/2;
        }
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _agent.SetDestination(dest);
        return GetPathTime(path);
    }

    public Vector3 RandomNavmeshLocation(float radius, Vector3 pos)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += pos;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    float GetPathTime(NavMeshPath path)
    {
        // Calculate the time to traverse the path
        float time = 0;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            // Get the start and end points of the segment
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i + 1];

            // Add the time to traverse the segment
            time += Vector3.Distance(start, end) / _agent.speed;
        }
        return time;
    }
}
