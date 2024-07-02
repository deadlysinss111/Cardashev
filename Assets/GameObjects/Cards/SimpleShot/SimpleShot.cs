using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleShot : Card
{
    List<GameObject> _selectableTiles = new();

    // Start is called before the first frame update
    void Awake()
    {
        int[] stats = new int[1] { 13 };
        base.Init(1.5f, 4, 50, stats, $"Take aim and perform a powerfull shot that deals {stats[0]} dmg.");

        while (PlayerManager.AddState("shot" + _id.ToString(), EnterAimState, ExitState) == false) _id++;

        if (TryGetComponent(out _selectableArea) == false)
            _selectableArea = gameObject.AddComponent<SelectableArea>();
    }

    public override void Effect()
    {
        if (_target == null) return;
        if (_target.TryGetComponent(out Enemy enemy) == false)
        {
            throw new MissingComponentException($"The object {_target.name} ({_target.GetType()}) the card aimed at does not have a Enemy script.");
        }
        base.Effect();
        enemy.TakeDamage(_stats[0]);
        GI._PlayerFetcher().GetComponent<PlayerManager>().SetToDefault();
    }

    void EnterAimState()
    {
        // Sets up the settings for the area
        _selectableArea.SetGroundColor(new Color(0.3f, 0.3f, 0.3f));
        _selectableArea.SetSelectableEntites(false, false, true, false);
        _selectableTiles = _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, 4);

        GI._PManFetcher().SetLeftClickTo(() => {
            if (_selectableArea.CastLeftClick(out GameObject obj))
            {
                print("You got hit by a smooth " + obj.name);
                _target = obj;
                base.ClickEvent();
            }
            else
                print("R u ok?");
        });
        GI._PManFetcher().SetRightClickTo(() => { ExitState(); GI._PManFetcher().SetToDefault(); });
        GI._PManFetcher().SetHoverTo(() => { });
    }

    void ExitState()
    {
        _selectableArea.ResetSelectable();
    }

    public override void ClickEvent()
    {
        GI._PManFetcher().SetToState("shot" + _id.ToString());
    }
}