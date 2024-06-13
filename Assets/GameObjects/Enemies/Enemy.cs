using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

abstract public class Enemy : MonoBehaviour
{
    /*
     FIELDS
    */
    // Ennemy action related
    protected NavMeshAgent _agent;
    protected GameObject _target;
    protected float _timeBeforeDecision;
    protected bool _isMoving;

    // Death related
    protected bool _waitForDestroy;
    protected ParticleSystem _particleSystem;

    // Allows the call of the death animation in place of the usual Act()
    Action _eff;

    /*
     EVENTS
    */
    public UnityEvent _UeOnDefeat;


    /*
     METHODS
    */
    private void Awake()
    {
        // Event subscribing
        _UeOnDefeat.AddListener(Defeat);
    }

    protected void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _target = GI._PlayerFetcher();
        _timeBeforeDecision = 0.0f;

        _particleSystem = GetComponent<ParticleSystem>();
        _waitForDestroy = false;

        _eff = Act;
    }

    void Update()
    {
        CheckPlayerDistance();
        // We update the timer, then if there is no action in progress, the enemy will decide to do something
        _timeBeforeDecision -= Time.deltaTime;
        if (_timeBeforeDecision <= 0)
        {
            _eff();
        }
    }

    // Pick a random reachable position
    public Vector3 RandomNavmeshLocation(float radius, Vector3 pos)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
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
    protected abstract void Act();

    protected abstract void Move();

    void CheckPlayerDistance()
    {
        // If the enemy is too close to the player, he will stop to move
        if( _isMoving && Vector3.Magnitude(_target.transform.position - transform.position) < 2)
        {
            _agent.destination = transform.position;
            _timeBeforeDecision = 0;
        }
    }

    private void Defeat()
    {
        _particleSystem.Play();
        _eff = ParticleHandle;

        // Ensures the animation plays out entirely
        _timeBeforeDecision = 0;
    }

    // Needs to be called every frame after defeat so that the GO is detroyed correctly after the animation
    private void ParticleHandle()
    {
        if (_particleSystem.isEmitting == false)
        {
            Color c = transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
            c.a = c.a - (1.0f * Time.deltaTime);
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = c;
        }
        if (_particleSystem.isStopped)
        {
            Destroy(gameObject);
        }
    }
}
