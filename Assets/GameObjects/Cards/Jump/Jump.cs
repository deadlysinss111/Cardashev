using UnityEngine;
using UnityEngine.AI;

public class Jump : Card
{
    Vector3 _initVelocity;
    Vector3 _lastDest;

    private void Awake()
    {
        int[] stats = new int[2] { 10, 4 };
        base.Init(2, 4, 40, stats, $"perform a jump to the target tile in a radius of {stats[0]} cases.");

        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState("jump" + _id.ToString(), EnterJumpState, ExitState) == false) _id++;

        if (TryGetComponent(out _selectableArea) == false)
            _selectableArea = gameObject.AddComponent<SelectableArea>();
    }

    void EnterJumpState()
    {
        _selectableArea.SetSelectableEntites(false, false, false, true);
        _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, _stats[0], _stats[1]);

        PlayerManager manager = GI._PManFetcher();
        manager.SetLeftClickTo(TriggerJump);
        manager.SetRightClickTo(() => { ExitState(); GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault(); });
        manager.SetHoverTo(Preview);
        GI.UpdateCursors("Jump", (byte)(GI.CursorRestriction.S_TILES));
        GI.UpdateCursorsInverted("Cross", (byte)(GI.CursorRestriction.S_TILES));
    }

    void ExitState()
    {
        _selectableArea.ResetSelectable();
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
        PlayerManager manager = GI._PManFetcher();
        // Crop the destination to the center of the target tile
        Vector3 alteredPos = manager._lastHit.transform.position;
        alteredPos.y += 0.5f;

        _lastDest = alteredPos;
        if (_selectableArea.CheckForSelectableTile(alteredPos) == false)
        {
            ClearPath();
            return;
        }

        Vector3 playerPos = manager._virtualPos;
        _initVelocity = TrajectoryToolbox.BellCurveInitialVelocity(playerPos, alteredPos, 5.0f);
        TrajectoryToolbox.BellCurve(playerPos, _initVelocity, ref _lineRenderer);
    }

    protected void TriggerJump()
    {
        if (_selectableArea.CheckForSelectableTile(_lastDest) == false) return;
        _selectableArea.ResetSelectable();
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
