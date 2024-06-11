using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableArea : MonoBehaviour
{
    public List<string> ignoreLayerList = new() { 
        "Player",
        "Interactable",
        "Enemy"
    };

    List<GameObject> selectableTiles;

    List<Vector3> debugRayStart;
    List<Vector3> debugRayDir;
    List<Color> debugRayColor;

    // Start is called before the first frame update
    void Start()
    {
        selectableTiles = new List<GameObject>();

        debugRayStart = new List<Vector3>();
        debugRayDir = new List<Vector3>();
        debugRayColor = new List<Color>();
    }

    // Update is called once per frame
    void Update()
    {
        if (debugRayStart.Count > 0 && (debugRayStart.Count == debugRayDir.Count))
        {
            for (int i = 0; i < debugRayStart.Count; i++)
            {
                Debug.DrawRay(debugRayStart[i], debugRayDir[i], debugRayColor[i]);
            }
        }

        ResetDebugRay();

        foreach (var tile in selectableTiles)
        {
            Vector3 pos = tile.transform.position;
            pos.y += 5;

            int layerMask = 0;
            foreach (var layer in ignoreLayerList)
            {
                if (layer == "Player") continue;
                layerMask |= 1 << LayerMask.NameToLayer(layer);
            }

            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.gameObject != tile)
                {
                    DebugRay(pos, Vector3.down * hit.distance, Color.green);
                    print($"Detected {hit.transform.gameObject.name} at pos [{pos.x}, {pos.z}]");
                }
                else
                {
                    DebugRay(pos, Vector3.down * hit.distance, Color.red);
                }
            }
            else
            {
                DebugRay(pos, Vector3.down * 100, Color.black);
            }
        }
    }

    public List<GameObject> FindSelectableArea(GameObject obj, int radius, int inner_radius, bool ignore_interactable=false)
    {
        ResetSelectable();

        int layerMask = 0;
        foreach (var layer in ignoreLayerList)
        {
            if (layer == "Interactable" && ignore_interactable) continue;
            layerMask |= 1 << LayerMask.NameToLayer(layer);
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

                Vector3 origin = pos + new Vector3(i * 1f, 0, j * 1f);

                //print("Radius check - " + i);
                RaycastHit hit;
                if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, layerMask))
                {
                    if (hit.transform.gameObject.CompareTag("TMTopology") == false)
                    {
                        DebugRay(origin, Vector3.down * hit.distance, Color.blue);
                        continue;
                    }

                    DebugRay(origin, Vector3.down * hit.distance, Color.yellow);
                    Debug.Log("Did Hit " + hit.transform.gameObject.name);
                    selectableTiles.Add(hit.transform.gameObject);
                    try
                    {
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 0, 1f);
                    }
                    catch (MissingComponentException e)
                    {
                        Debug.LogError($"An error occured when hitting {hit.transform.gameObject.name}: {e.ToString()}");
                    }
                }
                else
                {
                    DebugRay(origin, Vector3.down * 100, Color.red);
                    //Debug.DrawRay(origin, Vector3.down * 1000, Color.red);
                    Debug.Log("Did Hitn't");
                    //break;
                }
            }
        }
        return selectableTiles;
    }

    public List<GameObject> FindSelectableArea(GameObject obj, int radius, bool ignore_interactable = false)
    {
        ResetSelectable();

        int layerMask = 0;
        foreach (var layer in ignoreLayerList)
        {
            if (layer == "Interactable" && ignore_interactable) continue;
            layerMask |= 1 << LayerMask.NameToLayer(layer);
        }
        layerMask = ~layerMask;

        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                Vector3 pos = obj.transform.position;
                pos.y += 5;

                Vector3 origin = pos + new Vector3(i * 1f, 0, j * 1f);

                //print("Radius check - " + i);
                RaycastHit hit;
                if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, layerMask))
                {
                    if (hit.transform.gameObject.CompareTag("TMTopology") == false)
                    {
                        DebugRay(origin, Vector3.down * hit.distance, Color.blue);
                        continue;
                    }

                    DebugRay(origin, Vector3.down * hit.distance, Color.yellow);
                    Debug.Log("Did Hit " + hit.transform.gameObject.name);
                    selectableTiles.Add(hit.transform.gameObject);
                    try
                    {
                        hit.transform.gameObject.GetComponent<Tile>()._selectable = true;
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 0, 1f);
                    }
                    catch (MissingComponentException e)
                    {
                        Debug.LogError($"An error occured when hitting {hit.transform.gameObject.name}: {e}");
                    }
                }
                else
                {
                    DebugRay(origin, Vector3.down * 100, Color.red);
                    Debug.Log("Did Hitn't");
                    //break;
                }
            }
        }
        return selectableTiles;
    }

    public List<GameObject> GetSelectableTiles()
    {
        return selectableTiles;
    }

    void ResetSelectable()
    {
        ResetDebugRay();

        foreach (var tile in selectableTiles)
        {
            tile.GetComponent<Tile>()._selectable = false;
        }
        selectableTiles.Clear();
    }
    void ResetDebugRay()
    {
        debugRayStart.Clear();
        debugRayDir.Clear();
        debugRayColor.Clear();
    }

    void DebugRay(Vector3 origin, Vector3 direction)
    {
        debugRayStart.Add(origin);
        debugRayDir.Add(direction);
        debugRayColor.Add(Color.white);
    }
    void DebugRay(Vector3 origin, Vector3 direction, Color color)
    {
        debugRayStart.Add(origin);
        debugRayDir.Add(direction);
        debugRayColor.Add(color);
    }
}
