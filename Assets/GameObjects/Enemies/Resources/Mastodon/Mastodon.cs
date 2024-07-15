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
    int _dmg;
    GameObject[] _barrels;
    GameObject[] _diveSpots;

    private new void Start()
    {
        base.Start();
        _name = "Mastodon";
        _agent.speed = 5.5f;
        _dmg = 15;
        
    }

    // Enemy's decision
    override protected void Act()
    {
        _isMoving = false;

        _barrels = GameObject.FindGameObjectsWithTag("Spillable");
        _diveSpots = GameObject.FindGameObjectsWithTag("DiveSpot");

        GameObject closestBarrel = FindClosestBarrel(out float distToBarrel);
        if(distToBarrel <= 20)
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
            StartCoroutine(Swipe());
            return;
        }
        if (gameObject.GetComponent<StatManager>()._armor == 0)
        {
            if (Random.Range(0, 2) == 0)
            {
                StartCoroutine(ArmoreUp());
            }
            else
            {
                Move();
            }
        }
        else
        {
            Move();
        }
    }

    // Melee attack
    private IEnumerator Swipe()
    {
        _timeBeforeDecision = 3;
        // ANIM HERE
        Vector3 dir = Vector3.Normalize(new Vector3(_target.transform.position.x - transform.position.x, 0, _target.transform.position.z - transform.position.z));
        LookInDirectionTarget(dir, 8f);
        while (_timeBeforeDecision > 1)
        {
            yield return null;
        }
        Instantiate(_swipeHitbox, transform.position + dir * _swipeHitbox.transform.localScale.z * 0.75f, Quaternion.LookRotation(dir)).GetComponent<AOEVisual>()._dmg = _dmg;
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
        float closestDist = 100;
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

    void MoveToClosestBarrel()
    {
        _isMoving = true;

        _target = FindClosestBarrel(out float dist);
        Vector3 dest;
        dest = _target.transform.position;
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
        _target = GI._PlayerFetcher();
        barrel.tag = "Untagged";
        _timeBeforeDecision = 2;
    }


    //
    // Whole movement logic below
    //

    GameObject FindClosestDivSpotFromMe(out float time)
    {
        GameObject closestSpot = null;
        NavMeshPath path = new();
        float closestTime = 1000;
        foreach (GameObject spot in _diveSpots)
        {
            if(Physics.Raycast(spot.transform.position + new Vector3(0, 2f, 0) - Vector3.Normalize(spot.transform.position - transform.position) * 2, Vector3.down, out RaycastHit hit))
            {
                NavMesh.CalculatePath(transform.position, hit.transform.position, NavMesh.AllAreas, path);
                time = GetPathTime(path);
                if (time < closestTime && time != 0)
                {
                    closestSpot = spot;
                    closestTime = time;
                }
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
            NavMesh.CalculatePath(spot.transform.position + new Vector3(0, 0.5f, 0) - Vector3.Normalize(_target.transform.position - spot.transform.position) * 2, _target.transform.position, NavMesh.AllAreas, path);
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
        print("isnull? : "+ closestFromMe);
        if (closestFromMe == null || closestFromMe == closestFromTarget) return -1;

        NavMesh.CalculatePath(transform.position, closestFromMe.transform.position + new Vector3(0, 0.5f, 0) - Vector3.Normalize(closestFromMe.transform.position - transform.position) * 2, NavMesh.AllAreas, paths[0]);
        NavMesh.CalculatePath(
            closestFromTarget.transform.position + new Vector3(0, 0.5f, 0) - Vector3.Normalize(closestFromTarget.transform.position - _target.transform.position) * 2, 
            _target.transform.position + new Vector3(0, 0.5f, 0) ,
            NavMesh.AllAreas, paths[1]);
        print("time 1 : " + GetPathTime(paths[0]));
        print("time 2 : " + GetPathTime(paths[1]));

        return (GetPathTime(paths[0]) + GetPathTime(paths[1]));
    }

    IEnumerator MoveWithDive(NavMeshPath[] paths)
    {

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

        // Dive there
        _agent.enabled = false;
        transform.position = destTwo;
        _agent.enabled = true;

        _agent.SetDestination(destTwo);
        LookInDirectionTarget(destTwo, 8f);

        pathTime = GetPathTime(paths[1]);
        while (pathTime > 0)
        {
            pathTime -= Time.deltaTime;
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

    protected override void Move()
    {
        _isMoving = true;

        NavMeshPath directPath = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, _target.transform.position, NavMesh.AllAreas, directPath);

        float divePathTime = GetDivePath(out NavMeshPath[] divePaths);
        print("dive : "+ divePathTime);
        print("direct : "+ GetPathTime(directPath));
        if (divePathTime != -1 && divePathTime < GetPathTime(directPath))
        {
            _timeBeforeDecision = divePathTime;
            StartCoroutine(MoveWithDive(divePaths));
        }
        else
            MoveDirectly();
    }
}
