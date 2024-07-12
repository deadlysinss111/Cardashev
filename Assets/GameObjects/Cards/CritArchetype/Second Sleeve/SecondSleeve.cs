using UnityEngine;
using UnityEngine.Rendering.Universal;

// replace all TEMPLATE

public class SecondSleeve : Card
{
    Vector3 _direction;
    Vector3 _startingPosition;
    int _damage = 10;

    private void Awake()
    {
        // Call the Card Initialization method with arguments as following (duration, maxLvl, goldValue, Stats)
        int[] stats = new int[0];
        /* stats fill there */
        base.Init(1, 2, 60, stats);


        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState("SecondSleeve" + _id.ToString(), EnterState, ExitState) == false) _id++;

        if (TryGetComponent(out _selectableArea) == false)
            _selectableArea = gameObject.AddComponent<SelectableArea>();
    }

    void EnterState()
    {
        PlayerManager manager = GI._PManFetcher();

        // Card range
        _selectableArea.SetSelectableEntites(false, true, true, true);
        _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, 1, 0);

        manager.SetLeftClickTo(LeftClick);
        manager.SetRightClickTo(() => { ExitState(); GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault(); });
        manager.SetHoverTo(DisplayRange);
    }

    void LeftClick()
    {
        ClearPath();
        _selectableArea.ResetSelectable();
        PlayerManager manager = GI._PManFetcher();
        manager.SetToDefault();

        if (manager._lastHit.transform == null) return;

        _direction = Vector3.Normalize(manager._lastHit.transform.position - manager._virtualPos);
        _direction.y = 0;
        _startingPosition = manager._virtualPos;

        base.PlayCard();
    }

    void ExitState()
    {
        _selectableArea.ResetSelectable();
        ClearPath();
    }

    void DisplayRange()
    {

    }

    public override void Effect()
    {
        GameObject bullet = Instantiate((GameObject)Resources.Load("Bullet"));

        bullet.GetComponent<Bullet>().SetDirection(_direction);
        _startingPosition.y += 3;
        bullet.GetComponent<Bullet>().SetInitialValues(_startingPosition, 10, _damage);

        base.Effect();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("SecondSleeve" + _id.ToString());
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

}