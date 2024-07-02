using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Jump : Card
{
    Vector3 _initVelocity;
    Vector3 _lastDest;

    private void Awake()
    {
        int[] stats = new int[3];
        base.Init(2, 2, 60, stats);

        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState("jump" + _id.ToString(), EnterJumpState, ExitState) == false) _id++;
    }

    void EnterJumpState()
    {
        PlayerManager manager = GI._PManFetcher();
        manager.SetLeftClickTo(TriggerJump);
        manager.SetRightClickTo(() => { ExitState(); GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault(); });
        manager.SetHoverTo(Preview);
    }

    void ExitState()
    {
        //ClearPath();
    }

    public override void Effect()
    {
        GameObject player = GI._PlayerFetcher();

        // We need to disable the agent and setting the RigidBody to movable (not kinematik) in order to be able to manipulate rigidbody's velocity
        player.GetComponent<NavMeshAgent>().enabled = false;
        player.GetComponent<Rigidbody>().isKinematic = false;

        // The velocity is the last calcaulated one from the preview
        player.GetComponent<Rigidbody>().velocity = _initVelocity;

        // We need to set back the player to its normal state once it landed;
        player.AddComponent<AgentBackOnLanding>();

        base.Effect();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("jump" + _id.ToString());
    }

    private void Preview()
    {
        print("state entered");
        PlayerManager manager = GI._PManFetcher();
        // Crop the destination to the center of the target tile
        Vector3 alteredPos = manager._lastHit.transform.position;
        alteredPos.y += 0.5f;

        _lastDest = alteredPos;

        Vector3 playerPos = manager._virtualPos;
        _initVelocity = TrajectoryToolbox.BellCurveInitialVelocity(playerPos, alteredPos, 5.0f);
        TrajectoryToolbox.BellCurve(playerPos, _initVelocity, ref _lineRenderer);
    }

    protected void TriggerJump()
    {
        ClearPath();
        PlayerManager manager = GI._PManFetcher();
        manager._virtualPos = _lastDest;
        manager.SetToDefault();
        // Trigger the card play event
        base.PlayCard();
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }
}
