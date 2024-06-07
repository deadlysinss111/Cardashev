using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TirSimple : Card
{
    byte _id;

    // Start is called before the first frame update
    void Start()
    {
        _id = 0;
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        while (manager.AddState("shoot" + _id.ToString(), EnterAimState, ExitState) == false) _id++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EnterAimState()
    {
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        SetGroundColor(new Color(0.3f, 0.3f, 0.3f));
        FindSurrondingTiles(GameObject.FindGameObjectWithTag("Player"), 7);
        manager.SetRightClickTo(() => { ExitState(); GameObject.Find("Player").GetComponent<PlayerManager>().SetToDefault(); });
        manager.SetHoverTo(() => { });
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

    void FindSurrondingTiles(GameObject obj, int radius)
    {
        GameObject floorTiles = GameObject.FindGameObjectsWithTag("TMTopology").ToList()[0];
        for (int i = 0; i < floorTiles.transform.childCount; i++)
        {
            GameObject tile = floorTiles.transform.GetChild(i).gameObject;
            //tile.GetComponent<Tile>().
        }
    }
}
