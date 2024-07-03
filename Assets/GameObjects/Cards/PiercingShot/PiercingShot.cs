using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PiercingShot : Card
{
    List<GameObject> _selectableTiles = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        _duration = 4f;
        _stats = new int[2] { 6, 24 };
        base.Init(3, 4, 85, _stats, $"Take aim and perform a powerfull shot that deals {_stats[0]} dmg. Deals instead {_stats[1]} if it is a critical strike");

        while (PlayerManager.AddState("PiercingShot" + _id.ToString(), EnterAimState, ExitState) == false) _id++;

        if (TryGetComponent(out _selectableArea) == false)
            _selectableArea = gameObject.AddComponent<SelectableArea>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void Effect()
    {
        if (_target == null) return;
        if (_target.TryGetComponent(out Enemy enemy) == false)
        {
            throw new MissingComponentException($"The object {_target.name} ({_target.GetType()}) the card aimed at does not have a Enemy script.");
        }
        base.Effect();
        enemy.TakeDamage(_stats[GI._PlayerFetcher().GetComponent<StatManager>().HasCritical() ? 1 : 0]);
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
                _target = obj;
                GI._PlayerFetcher().GetComponent<PlayerManager>().SetToDefault();
                base.PlayCard();
            }
            else
                print("R u ok?");
        });
        manager.SetRightClickTo(() => { ExitState(); GI._PManFetcher().SetToDefault(); });
        manager.SetHoverTo(() => { });

        GI._cursor = (Texture2D)Resources.Load("Sword");
    }

    void ExitState()
    {
        _selectableArea.ResetSelectable();
        GI._cursor = null;
    }

    public override void PlayCard()
    {
        GI._PManFetcher().SetToState("PiercingShot" + _id.ToString());
    }
}