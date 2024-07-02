using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PiercingShot : Card
{
    List<GameObject> _selectableTiles = new List<GameObject>();

    SelectableArea AreaSelector;

    // Start is called before the first frame update
    void Start()
    {
        _duration = 4f;
        _stats = new int[2] { 6, 24 };
        int[] stats = new int[2];
        base.Init(3, 4, 85, stats, $"Take aim and perform a powerfull shot that deals {stats[0]} dmg. Deals instead {stats[1]} if it is a critical strike");

        while (PlayerManager.AddState("PiercingShot" + _id.ToString(), EnterAimState, ExitState) == false) _id++;

        if (TryGetComponent(out AreaSelector) == false)
            AreaSelector = gameObject.AddComponent<SelectableArea>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void Effect(GameObject obj)
    {
        base.Effect();
        if (obj.TryGetComponent(out Enemy enemy) == false)
        {
            throw new MissingComponentException($"The object {obj.name} ({obj.GetType()}) the card aimed at does not have a Enemy script.");
        }
        enemy.TakeDamage(_stats[GI._PlayerFetcher().GetComponent<StatManager>().HasCritical() ? 1 : 0]);
        base.ClickEvent(); // Calls this function to add the card to the queue
        GI._PManFetcher().SetToDefault();
    }

    void EnterAimState()
    {
        // Sets up the settings for the area
        AreaSelector.SetGroundColor(new Color(0.3f, 0.3f, 0.3f));
        AreaSelector.SetSelectableEntites(false, false, true, false);
        _selectableTiles = AreaSelector.FindSelectableArea(GI._PManFetcher()._virtualPos, 7);

        PlayerManager manager = GI._PManFetcher();
        manager.SetLeftClickTo(() => {
            if (AreaSelector.CastLeftClick(out GameObject obj))
            {
                print("You got hit by a smooth " + obj.name);
                Effect(obj);
            }
            else
                print("R u ok?");
        });
        manager.SetRightClickTo(() => { ExitState(); GI._PManFetcher().SetToDefault(); });
        manager.SetHoverTo(() => { });
    }

    void ExitState()
    {
        AreaSelector.ResetSelectable();
    }

    public override void ClickEvent()
    {
        GI._PManFetcher().SetToState("PiercingShot" + _id.ToString());
    }
}