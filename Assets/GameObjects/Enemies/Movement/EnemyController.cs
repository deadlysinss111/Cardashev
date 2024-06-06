using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Could probably be used later
    protected GameObject _player;

    protected BasicEnemyHandler _enemyHandler;
    [SerializeField] protected int _healthDanger;

    protected NavMeshAgent _agent;
    protected EnemyDeckManager _enemyDeckManager;

    protected float _queueTimer;

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
            Act();
        }
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

    protected float GetPathTime(NavMeshPath path)
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

    // Act is the location of the behaviour three
    protected virtual void Act() { }
}
