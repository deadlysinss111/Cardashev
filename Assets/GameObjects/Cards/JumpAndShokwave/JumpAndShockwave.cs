using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class JumpAndShockwave : Card
{
    private void Awake()
    {
        Dictionary<string, int> stats = new Dictionary<string, int>()
        {
            {"damage", 15},
            {"innerRange", 4},
            {"outerRange", 8}
        };
        base.Init("JumpAndShockwave", 3, 4, 80, stats, $"Jump to a nearby, dealing {stats["damage"]} dmg on landing", PreviewZoneType.ELLIPSIS);

        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState("jumpAndShockwave" + _id.ToString(), EnterJumpShockwaveState, ExitState) == false) _id++;

        if (TryGetComponent(out _selectableArea) == false)
            _selectableArea = gameObject.AddComponent<SelectableArea>();
    }

    void EnterJumpShockwaveState()
    {
        _selectableArea.SetSelectableEntites(false, false, false, true);
        _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, _stats["innerRange"], _stats["outerRange"]);

        PlayerManager manager = GI._PManFetcher();
        manager.SetLeftClickTo(TriggerJump);
        manager.SetRightClickTo(() => { 
            ExitState();
            GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault();
            if (_ghostHitbox != null)
                Destroy(_ghostHitbox);
        });
        manager.SetHoverTo(Preview);
        GI.UpdateCursors("Jump", (byte)(GI.CursorRestriction.S_TILES));
        GI.UpdateCursorsInverted("Cross", (byte)(GI.CursorRestriction.S_TILES));
    }

    new protected void Preview()
    {
        base.Preview();

        _ghostHitbox.transform.localScale = new Vector3(10, 2, 10);
    }

    void ExitState()
    {
        _selectableArea.ResetSelectable();
        ClearPath();
    }

    public override void Effect()
    {
        GameObject player = GI._PlayerFetcher();

        // We need to disable the agent and setting the RigidBody to movable (not kinematik) in order to be able to manipulate rigidbody's velocity
        player.GetComponent<NavMeshAgent>().enabled = false;
        player.GetComponent<Rigidbody>().isKinematic = false;

        // The velocity is the last calcaulated one from the preview
        player.GetComponent<Rigidbody>().velocity = _velocityFromLastBellCurveCalculated;

        // We need to set back the player to its normal state once it landed;
        player.AddComponent<AgentBackAndShockwaveOnLanding>();

        base.Effect();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("jumpAndShockwave" + _id.ToString());
    }

    //private void Preview()
    //{
    //    PlayerManager manager = GI._PManFetcher();
    //    // Crop the destination to the center of the target tile
    //    Vector3 alteredPos = manager._lastHit.transform.position;
    //    alteredPos.y += 0.5f;

    //    _previewRadius.transform.position = alteredPos;
    //    _lastDest = alteredPos;

    //    _lastDest = alteredPos;
    //    if (_selectableArea.CheckForSelectableTile(alteredPos) == false)
    //    {
    //        ClearPath();
    //        _previewRadius.SetActive(false);
    //        return;
    //    }
    //    _previewRadius.SetActive(true);

    //    Vector3 playerPos = manager._virtualPos;
    //    _initVelocity = TrajectoryToolbox.BellCurveInitialVelocity(playerPos, alteredPos, 5.0f);
    //    TrajectoryToolbox.BellCurve(playerPos, _initVelocity, ref _lineRenderer);
    //}

    protected void TriggerJump()
    {
        if (_selectableArea.CheckForSelectableTile(_destinationFromLastBellCurveCalculated) == false) return;
        //ClearPath();
        _selectableArea.ResetSelectable();
        GI._PManFetcher()._virtualPos = _destinationFromLastBellCurveCalculated;
        GI._PManFetcher().SetToDefault();
        // Trigger the card play event
        base.PlayCard();
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

    public override void OnLoad()
    {
        UnityEngine.Object RADIUS = Resources.Load("RadiusJumpPreview");
    }

    public override void OnUnload()
    {
    }
}
