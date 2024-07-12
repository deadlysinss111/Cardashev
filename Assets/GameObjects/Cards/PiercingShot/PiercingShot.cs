using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PiercingShot : Card
{
    List<GameObject> _selectableTiles = new List<GameObject>();
    Vector3 _direction;
    int _damage;

    void Awake()
    {
        _duration = 4f;
        int[] stats = new int[2] { 6, 24 };
        base.Init(3, 4, 85, stats, $"Take aim and perform a powerfull shot that deals {stats[0]} dmg. Deals {stats[1]} instead if it is a critical strike");

        while (PlayerManager.AddState("PiercingShot" + _id.ToString(), EnterState, ExitState) == false) _id++;

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
        Vector3 initPos = GI._PManFetcher()._virtualPos;
        initPos.y += 3;
        bullet.GetComponent<Bullet>().SetInitialValues(initPos, 100000, _stats[1]);

        base.Effect();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("PiercingShot" + _id.ToString());
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
    }

    /*public override void Effect()
    {
        if (_target == null) return;
        base.Effect();
        _target.GetComponent<Enemy>().TakeDamage(_stats[GI._PlayerFetcher().GetComponent<StatManager>().HasCritical() ? 1 : 0]);
    }

    void EnterAimState()
    {
        // Sets up the settings for the area
        _selectableArea.SetGroundColor(new Color(0.3f, 0.3f, 0.3f));
        _selectableArea.SetSelectableEntites(false, false, true, false);
        _selectableTiles = _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, 7);

        PlayerManager manager = GI._PManFetcher();
        manager.SetLeftClickTo(() => {
            if (_selectableArea.CastLeftClick(out GameObject obj))
            {
                print("You got hit by a smooth " + obj.name);
                if (obj.TryGetComponent(out Enemy _) == false)
                {
                    throw new MissingComponentException($"The object {obj.name} ({obj.GetType()}) the card aimed at does not have a Enemy script.");
                }
                _target = obj;
                GI._PlayerFetcher().GetComponent<PlayerManager>().SetToDefault();
                base.PlayCard();
            }
            else
                print("R u ok?");
        });
        manager.SetRightClickTo(() => { ExitState(); GI._PManFetcher().SetToDefault(); });
        manager.SetHoverTo(() => { });
        GI.UpdateCursors("Bow", (byte)GI.CursorRestriction.S_ENEMIES);
        GI.UpdateCursorsInverted("Cross", (byte)GI.CursorRestriction.S_ENEMIES);
    }
    
    void ExitState()
    {
        _selectableArea.ResetSelectable();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("PiercingShot" + _id.ToString());
    }*/
}