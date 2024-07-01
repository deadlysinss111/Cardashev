using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class JumpAndShockwave : Card
{
    Vector3 _initVelocity;
    Vector3 _lastDest;
    GameObject _previewRadius;

    private void Awake()
    {
        int[] stats = new int[3];
        base.Init(2, 2, 80, stats);

        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState("jumpAndShockwave" + _id.ToString(), EnterJumpShockwaveState, ExitState) == false) _id++;

        UnityEngine.Object RADIUS = Resources.Load("RadiusJumpPreview");
        _previewRadius = (GameObject)Instantiate(RADIUS);
        _previewRadius.SetActive(false);
    }

    void EnterJumpShockwaveState()
    {
        PlayerManager manager = GI._PManFetcher();
        manager.SetLeftClickTo(TriggerJump);
        manager.SetRightClickTo(() => { ExitState(); GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault(); });
        manager.SetHoverTo(Preview);
        _previewRadius.SetActive(true);
    }

    void ExitState()
    {
        _previewRadius.SetActive(false);
        ClearPath();
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
        player.AddComponent<AgentBackAndShockwaveOnLanding>();

        base.Effect();
    }

    public override void ClickEvent()
    {
        GI._PManFetcher().SetToState("jumpAndShockwave" + _id.ToString());
    }

    private void Preview()
    {
        PlayerManager manager = GI._PManFetcher();
        // Crop the destination to the center of the target tile
        Vector3 alteredPos = manager._lastHit.transform.position;
        alteredPos.y += 0.5f;

        _previewRadius.transform.position = alteredPos;
        _lastDest = alteredPos;

        Vector3 playerPos = manager._virtualPos;
        _initVelocity = TrajectoryToolbox.BellCurveInitialVelocity(playerPos, alteredPos, 5.0f);
        TrajectoryToolbox.BellCurve(playerPos, _initVelocity, ref _lineRenderer);
    }

    protected void TriggerJump()
    {
        ClearPath();
        GI._PManFetcher()._virtualPos = _lastDest;
        GI._PManFetcher().SetToDefault();
        // Trigger the card play event
        base.ClickEvent();
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }
}
