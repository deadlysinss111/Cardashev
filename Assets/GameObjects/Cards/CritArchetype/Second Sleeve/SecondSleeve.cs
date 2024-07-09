using UnityEngine;
using UnityEngine.Rendering.Universal;

// replace all TEMPLATE

public class SecondSleeve : Card
{
    Vector3 _direction;

    private void Awake()
    {
        // Call the Card Initialization method with arguments as following (duration, maxLvl, goldValue, Stats)
        int[] stats = new int[0];
        /* stats fill there */
        base.Init(2, 2, 60, stats);


        // Add a unique state + id to play the correct card and  not the first of its kind
        while (PlayerManager.AddState("SecondSleeve" + _id.ToString(), EnterState, ExitState) == false) _id++;
    }

    void EnterState()
    {
        PlayerManager manager = GI._PManFetcher();

        // Card range
        _selectableArea.SetSelectableEntites(false, true, true, true);
        _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, 0, 0);

        manager.SetLeftClickTo(() => { ClearPath(); _selectableArea.ResetSelectable(); GI._PManFetcher().SetToDefault(); base.PlayCard(); });
        manager.SetRightClickTo(() => { ExitState(); GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault(); });
        manager.SetHoverTo(DisplayRange);
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

        base.Effect();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("TEMPLATE" + _id.ToString());
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

}