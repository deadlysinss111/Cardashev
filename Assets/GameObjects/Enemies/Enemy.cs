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

    bool _selectable = false;
    public bool IsSelectable { get { return _selectable; } }

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

    public virtual void TakeDamage(int amount)
    {
        print($"Took {amount} damages!");
        _enemyHandler._stats.TakeDamage(amount);
    }

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

    /// <summary>
    /// Checks if the enemy is inside a set SelectableArea
    /// </summary>
    void CheckSelectable()
    {
        _selectable = false;
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.white; //Temp: Add an overlay or something later

        // If we aren't looking to select enemies, return
        if (SelectableArea.EnemyAreaCheck == false) return;

        // Filter out the player and the interactables from the Raycast
        int layerMask = (1 << LayerMask.NameToLayer("Player"));
        layerMask |= (1 << LayerMask.NameToLayer("Interactable"));
        layerMask = ~layerMask;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10, layerMask) == false) return;

        GameObject hitObj = hit.transform.gameObject;
        if (hitObj.TryGetComponent(out Tile tile) == false) return;

        //If the tile isn't among the selectable area, return
        if (tile.IsSelectable == false) return;

        SetSelected(true);
    }

    public void SetSelected(bool value)
    {
        _selectable = value;

        Outline outline;
        if (_selectable && TryGetComponent<Outline>(out _) == false)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = Color.red;
            outline.OutlineWidth = 7.7f;
        }
        else if (_selectable == false && TryGetComponent<Outline>(out outline))
        {
            Destroy(outline);
        }
    }
}
