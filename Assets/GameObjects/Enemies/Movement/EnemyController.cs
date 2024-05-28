using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Could probably be used later
    GameObject _player;

    NavMeshAgent _agent;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DecidePath()
    {
        
    }
}
