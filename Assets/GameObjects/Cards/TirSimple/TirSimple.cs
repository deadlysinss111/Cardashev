using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TirSimple : Card
{
    byte _id;

    // Temporary debug feature
    List<Vector3> debugRayStart = new List<Vector3>();
    List<Vector3> debugRayDir = new List<Vector3>();
    List<Color> debugRayColor = new List<Color>();

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
        // Temporary debug feature
        if (debugRayStart.Count > 0 && (debugRayStart.Count == debugRayDir.Count))
        {
            for (int i = 0; i < debugRayStart.Count; i++)
            {
                Debug.DrawRay(debugRayStart[i], debugRayDir[i], debugRayColor[i]);
            }
        }
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

    void FindSurrondingTiles(GameObject obj, int radius)
    {
        print("FindSurrondingTiles");
        /*GameObject floorTiles = GameObject.FindGameObjectsWithTag("TMTopology").ToList()[0];
        for (int i = 0; i < floorTiles.transform.childCount; i++)
        {
            GameObject tile = floorTiles.transform.GetChild(i).gameObject;
            //tile.GetComponent<Tile>().
        }*/

        Vector3[] dirs = {
            Vector3.forward,
            Vector3.back,
            Vector3.right,
            Vector3.left
        };

        // Add one to accomodates for the tile on the player feet
        for (int i = 0; i <= radius; i++)
        {
            foreach (Vector3 dir in dirs)
            {
                Vector3 origin = GameObject.Find("Player").transform.position + dir * (i);

                //print("Radius check - " + i);
                RaycastHit hit;
                if (Physics.Raycast(origin, Vector3.down, out hit))
                {
                    //Debug.DrawRay(origin, Vector3.down * hit.distance, Color.yellow);
                    debugRayStart.Add(origin); debugRayDir.Add(Vector3.down * 1000); debugRayColor.Add(Color.yellow);
                    Debug.Log("Did Hit " + hit.transform.gameObject.name);
                    //hit.transform.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 0, 1f);
                }
                else
                {
                    debugRayStart.Add(origin); debugRayDir.Add(Vector3.down * 1000); debugRayColor.Add(Color.red);
                    //Debug.DrawRay(origin, Vector3.down * 1000, Color.red);
                    Debug.Log("Did Hitn't");
                }
            }
        }
    }
}
