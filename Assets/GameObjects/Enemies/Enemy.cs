using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected GameObject _player;

    protected BasicEnemyHandler _enemyHandler;

    protected NavMeshAgent _agent;
    protected EnemyDeckManager _enemyDeckManager;

    protected float _queueTimer;

    protected GameObject _target;

    protected bool _isMoving;

    public bool _selectable = false;

    protected void Start()
    {
        _player = GameObject.Find("Player");
        _enemyHandler = GetComponent<BasicEnemyHandler>();
        _agent = GetComponent<NavMeshAgent>();
        _enemyDeckManager = GetComponent<EnemyDeckManager>();
        _target = _player;

        _enemyHandler._virtualPos = _agent.transform.position;
    }

    void Update()
    {
        CheckPlayerDistance();
        // We update the timer, then if there is no action in progress, the enemy will decide to do something
        _queueTimer -= Time.deltaTime;
        if (_queueTimer <= 0)
        {
            Act();
        }

        CheckSelectable();
    }

    // Pick a random reachable position
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

    protected virtual void Move() { }

    void CheckPlayerDistance()
    {
        // If the enemy is too close to the player, he will stop to move
        if( _isMoving && Vector3.Magnitude(_target.transform.position - transform.position) < 2)
        {
            _agent.destination = transform.position;
            _queueTimer = 0;
            print("break");
        }
    }

    void CheckSelectable()
    {
        _selectable = false;
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.white;

        if (SelectableArea.EnemyAreaCheck == false) return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10) == false) return;

        GameObject hitObj = hit.transform.gameObject;
        if (hitObj.TryGetComponent(out Tile tile) == false) return;

        if (tile._selectable == false) return;

        _selectable = true;


        if (_selectable)
        {
            print($"Enemy {gameObject.name} is selectable!");
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }
}
