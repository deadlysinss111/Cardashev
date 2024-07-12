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
        base.Effect();
        _target.GetComponent<Enemy>().TakeDamage(_stats[0]);
    }

    void EnterAimState()
    {
        // Sets up the settings for the area
        _selectableArea.SetGroundColor(new Color(0.3f, 0.3f, 0.3f));
        _selectableArea.SetSelectableEntites(false, false, true, false);
        _selectableTiles = _selectableArea.FindSelectableArea(GI._PManFetcher()._virtualPos, 4);

        PlayerManager manager = GI._PManFetcher();
        manager.SetLeftClickTo(() => {
            if (_selectableArea.CastLeftClick(out GameObject obj))
            {
                print("You got hit by a smooth " + obj.name);
                if (obj.TryGetComponent<Enemy>(out _) == false)
                {
                    throw new MissingComponentException($"The object {obj.name} ({obj.GetType()}) the card aimed at does not have a Enemy script.");
                }
                _target = obj;
                base.PlayCard();
                GI._PManFetcher().SetToDefault();
            }
            else
                print("R u ok?");
        });
        manager.SetRightClickTo(() => { ExitState(); GI._PManFetcher().SetToDefault(); });
        manager.SetHoverTo(() => { });
        GI.UpdateCursors("Bow", (byte)(GI.CursorRestriction.S_ENEMIES));
        GI.UpdateCursorsInverted("Cross", (byte)(GI.CursorRestriction.S_ENEMIES));
    }

    void ExitState()
    {
        _selectableArea.ResetSelectable();
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("shot" + _id.ToString());
    }
}