using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectableArea : MonoBehaviour
{
    [SerializeField] List<string> _ignoreLayerList = new() { 
        "Player",
        "Interactable",
        "Enemy"
    };

    List<GameObject> _selectableTiles;

    List<Vector3> _debugRayStart;
    List<Vector3> _debugRayDir;
    List<Color> _debugRayColor;

    [SerializeField] bool _allowSelectEnemy = true;
    [SerializeField] bool _allowSelectInteractable = true;
    [SerializeField] bool _allowSelectPlayer = false;
    [SerializeField] bool _allowSelectTiles = false;

    //[SerializeField] int _rayCastMaxDistance = 20;

    static bool _enemyAreaCheck;
    public static bool EnemyAreaCheck {  get { return _enemyAreaCheck; } }
    static bool _interactableAreaCheck;
    public static bool InteractableAreaCheck { get { return _interactableAreaCheck; } }

    // Start is called before the first frame update
    void Start()
    {
        _selectableTiles = new List<GameObject>();

        _debugRayStart = new List<Vector3>();
        _debugRayDir = new List<Vector3>();
        _debugRayColor = new List<Color>();

        _enemyAreaCheck = false;
        _interactableAreaCheck = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_debugRayStart.Count > 0 && (_debugRayStart.Count == _debugRayDir.Count))
        {
            for (int i = 0; i < _debugRayStart.Count; i++)
            {
                Debug.DrawRay(_debugRayStart[i], _debugRayDir[i], _debugRayColor[i]);
            }
        }

        // This is so unoptimized lmao
        /*foreach (var tile in _selectableTiles)
        {
            Vector3 pos = tile.transform.position;
            pos.y += 10;

            Vector3 scale = tile.transform.localScale;

            // Ready for 5 raycasts for every tile?
            Vector3[] posList =
            {
                new Vector3(pos.x, pos.y, pos.z),
                //new Vector3(pos.x + scale.x / 4, pos.y, pos.z - scale.z / 4),
                //new Vector3(pos.x - scale.x / 4, pos.y, pos.z + scale.z / 4),
                //new Vector3(pos.x + scale.x / 4, pos.y, pos.z + scale.z / 4),
                //new Vector3(pos.x - scale.x / 4, pos.y, pos.z - scale.z / 4),
            };

            int layerMask = 0;
            foreach (var layer in _ignoreLayerList)
            {
                if (layer == "Player" && _allowSelectPlayer == false) continue;
                if (layer == "Interactable" && _allowSelectInteractable == false) continue;
                if (layer == "Enemy" && _allowSelectEnemy == false) continue;
                layerMask |= (1 << LayerMask.NameToLayer(layer));
            }

            foreach (var origin in posList)
            {
                if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, _rayCastMaxDistance, layerMask))
                {
                    if (hit.transform.gameObject != tile)
                    {
                        DebugRay(origin, Vector3.down * hit.distance, Color.green);
                        print($"Detected {hit.transform.gameObject.name} at pos [{pos.x}, {pos.z}]");
                    }
                    else
                    {
                        DebugRay(origin, Vector3.down * hit.distance, Color.red);
                    }
                }
                else
                {
                    DebugRay(origin, Vector3.down * _rayCastMaxDistance, Color.black);
                }
                
            }
        }*/
    }

    public List<GameObject> FindSelectableArea(GameObject obj, int radius, int inner_radius, bool ignore_interactable=false)
    {
        ResetSelectable();

        int layerMask = 0;
        foreach (var layer in _ignoreLayerList)
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
                    _selectableTiles.Add(hit.transform.gameObject);
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
        if (_selectableTiles.Count > 0)
        {
            _enemyAreaCheck = _allowSelectEnemy;
            _interactableAreaCheck = _allowSelectInteractable;
        }
        return _selectableTiles;
    }

    public List<GameObject> FindSelectableArea(GameObject obj, int radius, bool ignore_interactable = false)
    {
        ResetSelectable();

        int layerMask = 0;
        foreach (var layer in _ignoreLayerList)
        {
            if (layer == "Interactable" && ignore_interactable) continue;
            layerMask |= 1 << LayerMask.NameToLayer(layer);
        }
        layerMask = ~layerMask;

        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                // Slighly change the x and z axis litteraly just becaue the player doesn't start in the center of a case currently
                // causing some raycasts to hit the same tile instead of each hitting their own
                Vector3 pos = obj.transform.position + new Vector3(0.25f, 5f, 0.25f);

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
                    _selectableTiles.Add(hit.transform.gameObject);
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
        if (_selectableTiles.Count > 0)
        {
            _enemyAreaCheck = _allowSelectEnemy;
            _interactableAreaCheck = _allowSelectInteractable;
        }
        return _selectableTiles;
    }

    public List<GameObject> GetSelectableTiles()
    {
        return _selectableTiles;
    }

    void ResetSelectable()
    {
        ResetDebugRay();

        foreach (var tile in _selectableTiles)
        {
            tile.GetComponent<Tile>()._selectable = false;
        }
        _selectableTiles.Clear();

        SetGroundColor(Color.white);

        _enemyAreaCheck = false;
        _interactableAreaCheck = false;
    }

    public void SetSelectableEntites(bool allowPlayer = false, bool allowInteractables = false, bool allowEnemies = true, bool allowTiles = true)
    {
        _allowSelectPlayer = allowPlayer;
        _allowSelectInteractable = allowInteractables;
        _allowSelectEnemy = allowEnemies;
        _allowSelectTiles = allowTiles;

        // If the enemies and/or the interactables can't be selected anymore but are currently raycasting, tell them to stop
        if (_allowSelectEnemy == false && _enemyAreaCheck)
            _enemyAreaCheck = false;
        if (_allowSelectInteractable == false && _interactableAreaCheck)
            _interactableAreaCheck = false;
    }

    public bool CastLeftClick(out GameObject obj, bool removeSelectable=true)
    {
        obj = null;

        int layerMask = 0;
        foreach (var layer in _ignoreLayerList)
        {
            if (layer == "Player" && _allowSelectPlayer) continue;
            if (layer == "Interactable" && _allowSelectInteractable) continue;
            if (layer == "Enemy" && _allowSelectEnemy) continue;
            layerMask |= (1 << LayerMask.NameToLayer(layer));
        }
        layerMask = ~layerMask;
        layerMask |= (1 << LayerMask.NameToLayer("Default"));

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, layerMask) == false) return false;

        GameObject objHit = hit.transform.gameObject;

        if (
            (objHit.TryGetComponent(out Tile tile) && tile._selectable && _allowSelectTiles) ||
            (objHit.TryGetComponent(out Enemy enemy) && enemy._selectable) ||
            (objHit.TryGetComponent(out Interactible interact) && interact._selectable)
            )
        {
            obj = objHit;
        }
        if (removeSelectable && obj != null)
            ResetSelectable();
        return obj != null;
    }

    // Temporary: to replace with an overlay or something similar
    public void SetGroundColor(Color color)
    {
        print("Yeh");
        List<GameObject> floorTiles = GameObject.FindGameObjectsWithTag("TMTopology").ToList();
        foreach (GameObject tile in floorTiles)
        {
            tile.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    void ResetDebugRay()
    {
        _debugRayStart.Clear();
        _debugRayDir.Clear();
        _debugRayColor.Clear();
    }

    void DebugRay(Vector3 origin, Vector3 direction)
    {
        _debugRayStart.Add(origin);
        _debugRayDir.Add(direction);
        _debugRayColor.Add(Color.white);
    }
    void DebugRay(Vector3 origin, Vector3 direction, Color color)
    {
        _debugRayStart.Add(origin);
        _debugRayDir.Add(direction);
        _debugRayColor.Add(color);
    }
}
