using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TirSimple : Card
{
    byte _id;

    List<GameObject> selectableTiles = new List<GameObject>();

    SelectableArea AreaSelector;
    PlayerManager Manager;

    // Start is called before the first frame update
    void Start()
    {
        _id = 0;
        Manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        while (Manager.AddState("shoot" + _id.ToString(), EnterAimState, ExitState) == false) _id++;

        if (TryGetComponent(out AreaSelector) == false)
            AreaSelector = gameObject.AddComponent<SelectableArea>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void EnterAimState()
    {
        SetGroundColor(new Color(0.3f, 0.3f, 0.3f));
        selectableTiles = AreaSelector.FindSelectableArea(GameObject.Find("Player"), 7);
        Manager.SetLeftClickTo(() => { });
        Manager.SetRightClickTo(() => { ExitState(); GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault(); });
        Manager.SetHoverTo(() => { });
    }

    void ExitState()
    {
        SetGroundColor(Color.white);
    }

    public override void ClickEvent()
    {
        GameObject.Find("Player").GetComponent<PlayerManager>().SetToState("shoot" + _id.ToString());
    }

    void SetGroundColor(Color color)
    {
        print("Yeh");
        List<GameObject> floorTiles = GameObject.FindGameObjectsWithTag("TMTopology").ToList();
        foreach (GameObject topology in floorTiles)
        {
            for (int i = 0; i < topology.transform.childCount; i++)
            {
                GameObject tile = topology.transform.GetChild(i).gameObject;
                tile.GetComponent<MeshRenderer>().material.color = color;
            }
        }
    }
}
