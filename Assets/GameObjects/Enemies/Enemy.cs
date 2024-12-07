using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

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
    protected Coroutine _lookAtCoroutine;
    protected float _lookRotationSpeed;
    protected Vector3 _targetLookAtVector;
    protected bool _lookAtTarget;

    // Death related
    [SerializeField] public int _health = 30;

    //// Particle systems
    [SerializeField] protected ParticleSystem _hurtParticles;
    [SerializeField] protected ParticleSystem _deathParticles;
    //// Everything else
    [SerializeField] protected Animator _animator;
    protected bool _waitForDestroy;
    protected string _name;
    private Type _type;
    public Type Type { get { return _type; } set { _type = value; } }

    // Selection related
    bool _selectable = false;
    public bool IsSelectable { get { CheckSelectable(); return _selectable; } }

    // Allows the call of the death animation in place of the usual Act()
    protected Action _eff;


    /*
     METHODS
    */
    protected void Awake()
    {
        // Event subscribing
        //_UeOnDefeat.AddListener(Defeat);
    }

    protected void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _target = GI._PlayerFetcher();
        _timeBeforeDecision = 0.0f;
        _lookRotationSpeed = 1.5f;

        _waitForDestroy = false;

        _eff = Act;
    }

    protected void Update()
    {
        CheckPlayerDistance();
        // We update the timer, then if there is no action in progress, the enemy will decide to do something
        _timeBeforeDecision -= Time.deltaTime;
        if (_timeBeforeDecision <= 0)
        {
            _eff();
        }

        CheckSelectable();
        UpdateFaceTarget();
        //print("Enemy is selectable: "+_selectable);
    }

    // Pick a random reachable position
    public Vector3 RandomNavmeshLocation(float radius, Vector3 pos)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += pos;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius*100, 1))
        {
            finalPosition = hit.position;
        }
        else
        {
            throw new Exception("sample position error");
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

    public void TakeDamage(int amount)
    {
        print($"Took {amount} damages!");
        StatManager manager = gameObject.GetComponent<StatManager>();
        manager.TakeDamage(amount);
        if (manager.Health <= 0) return;
        _hurtParticles.Play();
    }

    protected void CheckPlayerDistance()
    {
        // If the enemy is too close to the player, he will stop to move
        if( _isMoving && Vector3.Magnitude(GI._PlayerFetcher().transform.position - transform.position) < 2)
        {
            _agent.destination = transform.position;
            _timeBeforeDecision = 0;
        }
    }

    // Might be temporary? Might not be? Is less coroutine good or bad? Works a bit better than the coroutine version I think??
    protected void FaceTarget()
    {
        FaceTarget(_target.transform.position);
    }
    protected void FaceTarget(Vector3 target)
    {
        _lookAtTarget = true;
        _targetLookAtVector = target;
    }

    protected void UpdateFaceTarget()
    {
        if (_lookAtTarget == false) return;
        // Calculate the direction to the target destination
        Vector3 direction = (_targetLookAtVector - transform.position).normalized;

        // Rotate the player towards the target destination
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Smoothly rotate the player
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _lookRotationSpeed);

        if (
            Mathf.Approximately(transform.rotation.x, lookRotation.x) &&
            Mathf.Approximately(transform.rotation.y, lookRotation.y) &&
            Mathf.Approximately(transform.rotation.z, lookRotation.z)
       )
        {
            _lookAtTarget = false;
        }
    }

    public virtual void Defeat()
    {
        //TODO: ue there
        HierarchySearcher.FindChildRecursively(GameObject.Find("ExitTile").transform, "ExitePlate").GetComponent<EscapeTile>().TriggerCondition(_name);

        if (_lookAtCoroutine != null)
            StopCoroutine(_lookAtCoroutine);
        _agent.enabled = false;
        _isMoving = false;

        if (_animator != null)
            _animator.Play("Dying");
        _deathParticles.Play();
        _eff = ParticleHandle;

        // Ensures the animation plays out entirely
        _timeBeforeDecision = 0;
    }

    // Needs to be called every frame after defeat so that the GO is detroyed correctly after the animation
    protected void ParticleHandle()
    {
        if (_deathParticles.isStopped && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > _animator.GetCurrentAnimatorStateInfo(0).length/2) // Pretty shaky condition methinks
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Checks if the enemy is inside a set SelectableArea
    /// </summary>
    // TODO: giga opti => avoid chack at update; make tiles check on changestate and set a OnCollisionEnter or smth
    protected void CheckSelectable()
    {
        _selectable = false;

        // If we aren't looking to select enemies, return
        if (SelectableArea.EnemyAreaCheck == false)
        {
            if (TryGetComponent(out Outline outline))
                Destroy(outline);
            return;
        }

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
        // out _ means ou ignore the variable return by the out keyword
        if (_selectable && TryGetComponent<Outline>(out _) == false)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = Color.red;
            outline.OutlineWidth = 7.7f;
        }
        else if (_selectable == false && TryGetComponent(out outline))
        {
            Destroy(outline);
        }
    }

    [Obsolete("Not really deprecated but definitely have a problem with rotation that FaceTarget doesn't seem to have?? (Not as bad at least)")]
    protected void LookInDirectionTarget(Vector3 target, float speed)
    {
        if (_lookAtCoroutine != null)
            StopCoroutine(_lookAtCoroutine);
        _lookAtCoroutine = StartCoroutine(LookInDirectionTargetCoroutine(target, speed));
    }

    [Obsolete("Not really deprecated but definitely have a problem with rotation that FaceTarget doesn't seem to have?? (Not as bad at least)")]
    private IEnumerator LookInDirectionTargetCoroutine(Vector3 target, float speed)
    {
        Quaternion lookRotation = new();
        while (transform.rotation != lookRotation)
        {
            lookRotation = Quaternion.LookRotation(new Vector3(target.x, 0, target.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);
            yield return null;
        }
    }
}
