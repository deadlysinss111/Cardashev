using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EvilLaunchGrenade : LaunchGrenade
{
    CustomActions _input;
    [SerializeField] LayerMask _clickableLayers;
    Coroutine _waitForConfirmationCoroutine;
    List<Vector3> _pathPoints;


    LineRenderer _lineRenderer;

    Vector3 _grenadeInitVelocity;

    private void Awake()
    {
        _input = new CustomActions();
        _lineRenderer = GetComponent<LineRenderer>();
        _pathPoints = new List<Vector3>();
        _input.Enable();

        _duration = 2f;
    }

    public override void Effect(GameObject enemy)
    {
        FireGrenade(enemy);
    }

    protected void FireGrenade(GameObject enemy)
    {
        UnityEngine.Object GRENADE = Resources.Load("Grenade");
        GameObject grenade = (GameObject)Instantiate(GRENADE);
        Vector3 pos = enemy.GetComponent<BasicEnemyHandler>()._virtualPos;
        pos.y += 0.5f;
        grenade.GetComponent<Rigidbody>().transform.position = pos;
        grenade.GetComponent<Rigidbody>().velocity = TrailCalculator.BellCurveInititialVelocity(grenade.GetComponent<Rigidbody>().transform.position, GameObject.Find("Player").transform.position, 5);

        // Trigger the card play event
        //base.ClickEvent();
    }

    protected void StartCardAction(GameObject enemy)
    {
        GameObject grenade = (GameObject)Instantiate(Resources.Load("Grenade"));
        Vector3 pos = enemy.transform.position;
        pos.y = pos.y + 0.5f;
        grenade.transform.position = pos;
        grenade.GetComponent<Rigidbody>().velocity = TrailCalculator.BellCurveInititialVelocity(pos, GameObject.Find("Player").transform.position, Mathf.Clamp(Vector3.Distance(pos, GameObject.Find("Player").transform.position), 1f, 5f));
        BetterDebug.Log(grenade.GetComponent<Rigidbody>().velocity, grenade.transform.position);

        // Trigger the card play event
        //base.ClickEvent();
    }
}
