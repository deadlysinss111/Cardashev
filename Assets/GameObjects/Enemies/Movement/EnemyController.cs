using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Could probably be used later
    GameObject _player;

    BasicEnemyHandler _enemyHandler;
    [SerializeField] int _healthDanger;

    NavMeshAgent _agent;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
        _enemyHandler = GetComponent<BasicEnemyHandler>();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per framee
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DecidePath();
        }
    }

    void DecidePath()
    {
        Vector3 towardPlayer = _player.transform.position - transform.position;
        Vector3 awayPlayer = transform.position - towardPlayer;

        Vector3 dest;
        if (_enemyHandler.Health <  _healthDanger)
        {
            Debug.Log("Get away from player!");
            dest = RandomNavmeshLocation(4f, awayPlayer);
        }
        else
        {
            Debug.Log("Get closer to player!");
            dest = RandomNavmeshLocation(4f, towardPlayer);
        }
        _agent.SetDestination(dest);
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
}
