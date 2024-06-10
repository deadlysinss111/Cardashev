using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Jump : Card
{
    CustomActions _input;
    [SerializeField] LayerMask _clickableLayers;
    List<Vector3> _pathPoints;

    LineRenderer _lineRenderer;

    Vector3 _initVelocity;

    Vector3 _lastDest;

    byte _id;

    private void Awake()
    {
        _maxLv = 2;
        _stats = new int[3];
        _input = new CustomActions();
        _lineRenderer = GetComponent<LineRenderer>();
        _pathPoints = new List<Vector3>();
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _id = 0;
        while (manager.AddState("jump" + _id.ToString(), EnterJumpState, ExitState) == false) _id++;
        _goldValue = 60;
        _duration = 2;
    }

    void EnterJumpState()
    {
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        manager.SetLeftClickTo(TriggerJump);
        manager.SetRightClickTo(() => { ExitState(); GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault(); });
        manager.SetHoverTo(Preview);
    }

    void ExitState()
    {
        ClearPath();
    }

    public override void Effect()
    {
        GameObject player = GameObject.Find("Player");

        // We need to disable the agent and setting the RigidBody to movable (not kinematik) in order to be able to manipulate rigidbody's velocity
        player.GetComponent<NavMeshAgent>().enabled = false;
        player.GetComponent<Rigidbody>().isKinematic = false;

        // The velocity is the last calcaulated one from the preview
        player.GetComponent<Rigidbody>().velocity = _initVelocity;

        // We need to set back the player to its normal state once it landed;
        player.AddComponent<AgentBackOnLanding>();

        base.Effect();
    }

    public override void ClickEvent()
    {
        GameObject.Find("Player").GetComponent<PlayerManager>().SetToState("jump" + _id.ToString());
    }

    private void Preview()
    {
        print("state entered");
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        // Crop the destination to the center of the target tile
        Vector3 alteredPos = manager._lastHit.transform.position;
        alteredPos.y += 0.5f;

        _lastDest = alteredPos;

        Vector3 playerPos = manager._virtualPos;
        _initVelocity = TrailCalculator.BellCurveInitialVelocity(playerPos, alteredPos, 5.0f);
        TrailCalculator.BellCurve(playerPos, _initVelocity, ref _lineRenderer, out _pathPoints);
    }

    protected void TriggerJump()
    {
        ClearPath();
        GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos = _lastDest;
        GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault();
        // Trigger the card play event
        base.ClickEvent();
    }

    void ClearPath()
    {
        _lineRenderer.positionCount = 0;
    }

    // Enable input actions
    void OnEnable()
    {
        _input.Enable();
    }

    // Disable input actions
    void OnDisable()
    {
        _input.Disable();
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }
}
