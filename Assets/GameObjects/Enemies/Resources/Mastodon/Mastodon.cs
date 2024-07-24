using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Mastodon : Enemy
{
    [SerializeField] GameObject _swipeHitbox;
    [SerializeField] GameObject _biteHitbox;
    int _dmg;
    float _diveTime = 2;
    GameObject[] _barrels;
    GameObject[] _diveSpots;

    float _divePathTime;
    NavMeshPath[] _divePaths;
    bool _isDiveShorter;
    Vector3 _nextMoveDirection;

    private new void Start()
    {
        base.Start();
        _name = "Mastodon";
        _agent.speed = 2f;
        _dmg = 15;
        
    }

    // Enemy's decision
    override protected void Act()
    {
        _isMoving = false;

        _barrels = GameObject.FindGameObjectsWithTag("Spillable");
        _diveSpots = GameObject.FindGameObjectsWithTag("DiveSpot");

        IsDiveShorstestThanDirectWalk();

        GameObject closestBarrel = FindClosestBarrelWithAngleRestriction(_target.transform.position, 30,  out float distToBarrel);
        //GameObject closestBarrel = FindClosestBarrel(out float distToBarrel);
        if(distToBarrel < Vector3.Magnitude(_target.transform.position - transform.position))
        {
            Barrel barrelScript = HierarchySearcher.FindChildRecursively(closestBarrel.transform, "Body").GetComponent<Barrel>();
            if (barrelScript._targetedBy == null || barrelScript._targetedBy == gameObject)
            {
                if (Vector3.Magnitude(closestBarrel.transform.position - transform.position) <= 4)
                    SpillBarrel(closestBarrel);
                else
                {
                    barrelScript._targetedBy = gameObject;
                    MoveToClosestBarrel();
                }
                return;
            }
        }

        float dist = Vector3.Magnitude(_target.transform.position - transform.position);
        if (dist < 4)
        {
            if (Random.Range(0, 3) == 0)
                StartCoroutine(Bite());
            else
                StartCoroutine(Swipe());

            return;
        }
        Move();
    }

    // Melee attacks
    private IEnumerator Swipe()
    {
        _timeBeforeDecision = 3;
        // ANIM HERE
        Vector3 dir = Vector3.Normalize(new Vector3(_target.transform.position.x - transform.position.x, 0, _target.transform.position.z - transform.position.z));
        LookInDirectionTarget(dir, 8f);
        _animator.Play("Melee");
        while (_timeBeforeDecision > 1)
        {
            yield return null;
        }
        Instantiate(_swipeHitbox, transform.position + dir * _swipeHitbox.transform.localScale.z * 0.75f, Quaternion.LookRotation(dir)).GetComponent<Swipe>()._dmg = _dmg;
    }

    private IEnumerator Bite()
    {
        _timeBeforeDecision = 3.5f;
        // ANIM HERE
        Vector3 dir = Vector3.Normalize(new Vector3(_target.transform.position.x - transform.position.x, 0, _target.transform.position.z - transform.position.z));
        LookInDirectionTarget(dir, 8f);
        while (_timeBeforeDecision > 1.5f)
        {
            yield return null;
        }
        Instantiate(_biteHitbox, transform.position + dir * _biteHitbox.transform.localScale.z * 0.75f, Quaternion.LookRotation(dir)).GetComponent<Bite>()._dmg = _dmg;
    }

    // Gain armor
    private IEnumerator ArmoreUp()
    {
        print("armoring up");
        _timeBeforeDecision = 3;
        // ANIM HERE
        while (_timeBeforeDecision > 1)
        {
            yield return null;
        }
        gameObject.GetComponent<StatManager>()._armor += Random.Range(14, 17);
    }

    public void AcideBuff()
    {
        _dmg = 25;
        StartCoroutine(HealSelf(Random.Range(5, 8)));
        StartCoroutine(SetBackDmg(Random.Range(5, 7)));
    }

    IEnumerator HealSelf(float remainingLoops)
    {
        while(remainingLoops > 0)
        {
            gameObject.GetComponent<StatManager>().Heal(2);
            yield return new WaitForSeconds(1);
            remainingLoops -= 1;
        }
    }

    IEnumerator SetBackDmg(float timeLeft)
    {
        while(timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        _dmg = 15;
    }

    GameObject FindClosestBarrel(out float dist)
    {
        GameObject closestBarrel = null;
        float closestDist = 1000;
        foreach(GameObject barrel in _barrels)
        {
            dist = Vector3.Magnitude(barrel.transform.position - transform.position);
            if( dist < closestDist)
            {
                closestBarrel = barrel;
                closestDist = dist;
            }
        }
        dist = closestDist;
        return closestBarrel;
    }

    // This function is meant to find the closest barrel that is not too far from the enemy to target path
    // degree is the margin of permission the angle between the barrel and the targe tmust have
    GameObject FindClosestBarrelWithAngleRestriction(Vector3 targetPos, float degree, out float dist)
    {
        GameObject closestBarrel = null;
        float closestDist = 1000;
        foreach (GameObject barrel in _barrels)
        {
            if (Vector3.Angle(barrel.transform.position - transform.position, _nextMoveDirection) > degree)
                continue;

            dist = Vector3.Magnitude(barrel.transform.position - transform.position);
            if (dist < closestDist)
            {
                closestBarrel = barrel;
                closestDist = dist;
            }
        }
        dist = closestDist;
        return closestBarrel;
    }

    void MoveToClosestBarrel()
    {
        _isMoving = true;

        GameObject target = FindClosestBarrel(out float dist);
        Vector3 dest;
        dest = target.transform.position;
        dest -= 2* Vector3.Normalize(dest - transform.position);
        dest += new Vector3(0, 2, 0);

        Physics.Raycast(dest, Vector3.down, out RaycastHit hit);
        dest = hit.transform.position + new Vector3(0.5f, 0.5f, 0.5f);

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _agent.SetDestination(dest);
        LookInDirectionTarget(dest, 8f);

        _timeBeforeDecision = GetPathTime(path);
    }

    void SpillBarrel(GameObject barrel)
    {
        GameObject body = HierarchySearcher.FindChildRecursively(barrel.transform, "Body");
        body.GetComponent<Barrel>().Spill(barrel.transform.position - transform.position);
        barrel.tag = "Untagged";
        _timeBeforeDecision = 2;
    }


    //
    // Whole movement logic below
    //

    GameObject FindClosestTileFromTargetArg(List<GameObject> competings, GameObject target, out float dist)
    {
        GameObject closest = null;
        dist = 1000;
        foreach (GameObject competing in competings)
        {
            if(Vector3.Magnitude(competing.transform.position - target.transform.position) < dist)
            {
                dist = Vector3.Magnitude(competing.transform.position - target.transform.position);
                closest = competing;
            }
        }
        return closest;
    }

    GameObject FindClosestDivSpotFromMe(out float time)
    {
        GameObject closestSpot = null;
        NavMeshPath path = new();
        float closestTime = 1000;
        foreach (GameObject spot in _diveSpots)
        {
            NavMesh.CalculatePath(transform.position, FindClosestTileFromTargetArg(spot.GetComponent<DiveSpot>()._linkedTiles, _target, out _).transform.position, NavMesh.AllAreas, path);
            time = GetPathTime(path);
            if (time < closestTime && time != 0)
            {
                closestSpot = spot;
                closestTime = time;
            }
        }
        time = closestTime;
        //print("time : "+time);
        return closestSpot;
    }

    GameObject FindClosestDivSpotFromTarget(out float time)
    {
        GameObject closestSpot = null;
        NavMeshPath path = new();
        float closestTime = 1000;
        foreach (GameObject spot in _diveSpots)
        {
            NavMesh.SamplePosition(FindClosestTileFromTargetArg(spot.GetComponent<DiveSpot>()._linkedTiles, _target, out _).transform.position, out NavMeshHit firstHit, 10, NavMesh.AllAreas);
            NavMesh.SamplePosition(_target.transform.position, out NavMeshHit secondHit, 10, NavMesh.AllAreas);
            NavMesh.CalculatePath(firstHit.position, secondHit.position, NavMesh.AllAreas, path);

            time = GetPathTime(path);
            if (time < closestTime && time != 0)
            {
                closestSpot = spot;
                closestTime = time;
            }
        }
        time = closestTime;
        return closestSpot;
    }

    float GetDivePath(out NavMeshPath[] paths)
    {
        paths = new NavMeshPath[2];
        paths[0] = new NavMeshPath();
        paths[1] = new NavMeshPath();
        GameObject closestFromMe = FindClosestDivSpotFromMe(out _);
        GameObject closestFromTarget = FindClosestDivSpotFromTarget(out _);
        //print("isnull? : "+ closestFromMe);
        //print("isnull2? : "+ closestFromTarget);
        if (closestFromMe == null || closestFromMe == closestFromTarget) return -1;

        NavMesh.CalculatePath(transform.position, closestFromMe.transform.position + new Vector3(0, 0.5f, 0) - Vector3.Normalize(closestFromMe.transform.position - transform.position) * 2, NavMesh.AllAreas, paths[0]);
        NavMesh.CalculatePath(
            closestFromTarget.transform.position + new Vector3(0, 0.5f, 0) - Vector3.Normalize(closestFromTarget.transform.position - _target.transform.position) * 2, 
            _target.transform.position + new Vector3(0, 0.5f, 0) ,
            NavMesh.AllAreas, paths[1]);
        //print("time 1 : " + GetPathTime(paths[0]));
        //print("time 2 : " + GetPathTime(paths[1]));

        return (GetPathTime(paths[0]) + GetPathTime(paths[1]) + _diveTime);
    }

    IEnumerator MoveWithDive(NavMeshPath[] paths)
    {
        float animWaitTime;

        Vector3 destOne, destTwo;
        GameObject spotOne = FindClosestDivSpotFromMe(out _);
        destOne = spotOne.transform.position + new Vector3(0, 0.5f, 0) - Vector3.Normalize(spotOne.transform.position - transform.position)*4;
        destTwo = FindClosestDivSpotFromTarget(out _).transform.position + new Vector3(0, 0.5f, 0);

        _agent.SetDestination(destOne);
        LookInDirectionTarget(destOne, 8f);

        float pathTime = GetPathTime(paths[0]);
        while(pathTime > 0)
        {
            pathTime -= Time.deltaTime;
            yield return null;
        }

        pathTime = _diveTime / 2;
        while (pathTime > 0)
        {
            pathTime -= Time.deltaTime;
            yield return null;
        }

        _animator.Play("DiveDown");

        yield return new WaitForNextFrameUnit();

        animWaitTime = _animator.GetCurrentAnimatorStateInfo(0).length;
        while (AnimatorHelper.GetAnimationCurrentTime(_animator) <= animWaitTime)
        {
            yield return null;
        }

        // Dive there
        _agent.enabled = false;
        transform.position = destTwo;
        _agent.enabled = true;

        pathTime = _diveTime / 2;
        while (pathTime > 0)
        {
            pathTime -= Time.deltaTime;
            yield return null;
        }

        _agent.SetDestination(destTwo);
        LookInDirectionTarget(destTwo, 8f);

        pathTime = GetPathTime(paths[1]);
        while (pathTime > 0)
        {
            pathTime -= Time.deltaTime;
            yield return null;
        }

        _animator.Play("DiveUp");

        animWaitTime = _animator.GetCurrentAnimatorStateInfo(0).length;
        while (AnimatorHelper.GetAnimationCurrentTime(_animator) <= animWaitTime)
        {
            yield return null;
        }
    }

    protected void MoveDirectly()
    {
        Vector3 dest;
        dest = _target.transform.position;
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _agent.SetDestination(dest);
        LookInDirectionTarget(dest, 8f);

        _timeBeforeDecision = GetPathTime(path);

        // Once the half of the movement done, we want the enemy to refresh his destination to match target's movments
        // TODO: Decide of a tile to snap too, to ensure we don't stop in the middle of the board
        StartCoroutine(reOrient(_timeBeforeDecision / 2));
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Walking") == false)
            _animator.Play("Walking");
    }

    private IEnumerator reOrient(float time)
    {
        while (_timeBeforeDecision > time)
        {
            yield return null;
        }

        Vector3 dest = _target.transform.position;
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
        _agent.SetDestination(dest);
        LookInDirectionTarget(dest, 8f);
    }

    void IsDiveShorstestThanDirectWalk()
    {
        NavMeshPath directPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, _target.transform.position, NavMesh.AllAreas, directPath);

        _divePathTime = GetDivePath(out _divePaths);

        if (_divePathTime != -1 && _divePathTime < GetPathTime(directPath) && _divePaths[0].corners.Length != 0)
        {
            _nextMoveDirection = _divePaths[0].corners[_divePaths[0].corners.Length - 1] - transform.position;
            _isDiveShorter =  true;
            return;
        }

        _nextMoveDirection = _target.transform.position - transform.position;
        _isDiveShorter = false;
    }

    protected override void Move()
    {
        _isMoving = true;

        if (_isDiveShorter)
        {
            _timeBeforeDecision = _divePathTime + (AnimatorHelper.GetAnimationLength(_animator, "DiveDown") + AnimatorHelper.GetAnimationLength(_animator, "DiveUp"));
            StartCoroutine(MoveWithDive(_divePaths));
        }
        else
            MoveDirectly();
    }
}
