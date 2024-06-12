using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TirSimple : Card
{
    byte _id;

    List<GameObject> _selectableTiles = new List<GameObject>();

    SelectableArea AreaSelector;
    PlayerManager Manager;

    // Start is called before the first frame update
    void Start()
    {
        _duration = 1f;
        _id = 0;
        _stats = new int[1] { 13 };
        Manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        while (Manager.AddState("shoot" + _id.ToString(), EnterAimState, ExitState) == false) _id++;

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
        _selectableTiles = AreaSelector.FindSelectableArea(GameObject.Find("Player"), 4);

        Manager.SetLeftClickTo(() => {
            if (AreaSelector.CastLeftClick(out GameObject obj))
            {
                print("You got hit by a smooth " + obj.name);
                Effect(obj);
            }
            else
                print("R u ok?");
        });
        Manager.SetRightClickTo(() => { ExitState(); GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault(); });
        Manager.SetHoverTo(() => { });
    }

    void ExitState()
    {
        AreaSelector.SetGroundColor(Color.white);
    }

    public override void ClickEvent()
    {
        GameObject.Find("Player").GetComponent<PlayerManager>().SetToState("shoot" + _id.ToString());
    }
}