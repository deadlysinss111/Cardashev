using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleShot : Card
{
    List<GameObject> _selectableTiles = new();

    SelectableArea AreaSelector;

    // Start is called before the first frame update
    void Awake()
    {
        int[] stats = new int[1] { 13 };
        base.Init(1, 3, 50, stats);

        while (GI._PManFetcher().AddState("shot" + _id.ToString(), EnterAimState, ExitState) == false) _id++;

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
        enemy.TakeDamage(_stats[0]);
        base.ClickEvent(); // Calls this function to add the card to the queue
        GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault();
    }

    void EnterAimState()
    {
        // Sets up the settings for the area
        AreaSelector.SetGroundColor(new Color(0.3f, 0.3f, 0.3f));
        AreaSelector.SetSelectableEntites(false, false, true, false);
        _selectableTiles = AreaSelector.FindSelectableArea(GI._PManFetcher()._virtualPos, 4);

        GI._PManFetcher().SetLeftClickTo(() => {
            if (AreaSelector.CastLeftClick(out GameObject obj))
            {
                print("You got hit by a smooth " + obj.name);
                Effect(obj);
            }
            else
                print("R u ok?");
        });
        GI._PManFetcher().SetRightClickTo(() => { ExitState(); GI._PManFetcher().SetToDefault(); });
        GI._PManFetcher().SetHoverTo(() => { });
    }

    void ExitState()
    {
        AreaSelector.ResetSelectable();
    }

    public override void ClickEvent()
    {
        GI._PManFetcher().SetToState("shot" + _id.ToString());
    }
}