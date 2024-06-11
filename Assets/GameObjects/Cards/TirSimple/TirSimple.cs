using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class TirSimple : Card
{
    byte _id;

    // Temporary debug feature
    List<Vector3> debugRayStart = new List<Vector3>();
    List<Vector3> debugRayDir = new List<Vector3>();
    List<Color> debugRayColor = new List<Color>();

    List<GameObject> selectableTiles = new List<GameObject>();

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

        foreach (var tile in selectableTiles)
        {
            Vector3 pos = tile.transform.position;
            pos.y += 5;

            int layerMask = 1 << LayerMask.NameToLayer("Player");
            layerMask = ~layerMask;

            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.gameObject != tile)
                {
                    print($"Detected {hit.transform.gameObject.name} at pos [{pos.x}, {pos.z}]");
                }
            }
        }
    }

    void EnterAimState()
    {
        PlayerManager manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        SetGroundColor(new Color(0.3f, 0.3f, 0.3f));
        selectableTiles = FindSurrondingTiles(GameObject.FindGameObjectWithTag("Player"), 4, 0, false);
        manager.SetLeftClickTo(() => { });
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

    List<GameObject> FindSurrondingTiles(GameObject obj, int radius, int inner_radius=0, bool ignore_interactable=true)
    {
        print("FindSurrondingTiles");

        Vector3[] dirs = {
            Vector3.forward,
            Vector3.back,
            Vector3.right,
            Vector3.left
        };

        List<GameObject> list = new();

        int layerMask = 1 << LayerMask.NameToLayer("Player");
        if (ignore_interactable)
        {
            layerMask |= (1 << LayerMask.NameToLayer("Interactable"));
        }
        layerMask = ~layerMask;

        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            { 
                if (Mathf.Abs(i) <= inner_radius && Mathf.Abs(j) <= inner_radius)
                {
                    continue;
                }

                Vector3 pos = obj.transform.position;
                pos.y += 5;

                Vector3 origin = pos + new Vector3(i*1f, 0, j*1f);

                //print("Radius check - " + i);
                RaycastHit hit;
                if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, layerMask))
                {
                    if (hit.transform.gameObject.CompareTag("TMTopology") == false)
                    {
                        debugRayStart.Add(origin); debugRayDir.Add(Vector3.down * hit.distance); debugRayColor.Add(Color.blue);
                        continue;
                    }

                    debugRayStart.Add(origin); debugRayDir.Add(Vector3.down * hit.distance); debugRayColor.Add(Color.yellow);
                    Debug.Log("Did Hit " + hit.transform.gameObject.name);
                    list.Add(hit.transform.gameObject);
                    try
                    {
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 0, 1f);
                    } catch (MissingComponentException e)
                    {
                        Debug.LogError($"An error occured when hitting {hit.transform.gameObject.name}: {e.ToString()}");
                    }
                }
                else
                {
                    debugRayStart.Add(origin); debugRayDir.Add(Vector3.down * 100); debugRayColor.Add(Color.red);
                    //Debug.DrawRay(origin, Vector3.down * 1000, Color.red);
                    Debug.Log("Did Hitn't");
                    //break;
                }
            }
        }
        return list;
    }
}
