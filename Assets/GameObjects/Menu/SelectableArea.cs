using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectableArea : MonoBehaviour
{
    // The layers that can be used in the LayerMask. Shouldn't have a reason to change unless we rename those layers
    readonly List<string> _ignoreLayerList = new() {
        "Player",
        "Interactable",
        "Enemy"
    };

    // Currently unused (actually not)
    List<GameObject> _selectableTiles;

    RaycastHit[] _hitBuffer;
    RaycastHit[] _innerHitBuffer;

    // Used to draw the raincast
    List<Vector3> _debugRayStart;
    List<Vector3> _debugRayDir;
    List<Color> _debugRayColor;

    // What should the player be allowed to select
    [SerializeField] bool _allowSelectEnemy = true;
    [SerializeField] bool _allowSelectInteractable = true;
    [SerializeField] bool _allowSelectPlayer = false;
    [SerializeField] bool _allowSelectTiles = false;

    //[SerializeField] int _rayCastMaxDistance = 20;

    // Static variables used to tell the enemies and interactables to start raycasting
    static bool _enemyAreaCheck;
    public static bool EnemyAreaCheck { get { return _enemyAreaCheck; } }
    static bool _interactableAreaCheck;
    public static bool InteractableAreaCheck { get { return _interactableAreaCheck; } }

    void Start()
    {
        _selectableTiles = new List<GameObject>();

        _debugRayStart = new List<Vector3>();
        _debugRayDir = new List<Vector3>();
        _debugRayColor = new List<Color>();

        _enemyAreaCheck = false;
        _interactableAreaCheck = false;

        _hitBuffer = new RaycastHit[255];
        _innerHitBuffer = new RaycastHit[255];
    }

    void Update()
    {
        // Le RainCast
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

    // TODO FindSelectableArea: Fix inner radius

    /// <summary>
    /// Sets up an area of a defined radius around obj where objects can be selected
    /// </summary>
    /// <param name="obj">The object around which the area would be set</param>
    /// <param name="radius">The radius of the area</param>
    /// <param name="inner_radius">How many cases from inside should be excluded. DO NOT COUNT THE RADIUS</param>
    /// <param name="ignore_interactable">Whether the area should include the tiles below interactables or not</param>
    /// <returns>A list of tiles that are part of the selectable area</returns>
    public List<GameObject> FindSelectableArea(GameObject obj, int radius, int inner_radius)
    {
        if (inner_radius >= radius)
        {
            throw new ArgumentException($"[SelectableArea] The inner radius ({inner_radius}) should not be equal or bigger than the radius ({radius})!");
        }
        ResetSelectable();

        // Prevents players, interactables (if decided so) and enemies to be counted in the raycast
        int layerMask = 0;
        foreach (var layer in _ignoreLayerList)
        {
            layerMask |= 1 << LayerMask.NameToLayer(layer);
        }
        layerMask = ~layerMask;

        /*for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                // If it's part of the inner radius, leave it be
                if (Mathf.Abs(i) <= inner_radius && Mathf.Abs(j) <= inner_radius)
                {
                    continue;
                }

                // Slighly change the x and z axis litteraly just becaue the player doesn't start in the center of a case currently
                // causing some raycasts to hit the same tile instead of each hitting their own
                Vector3 pos = obj.transform.position + new Vector3(0.25f, 5f, 0.25f);

                Vector3 origin = pos + new Vector3(i * 1f, 0, j * 1f);

                //print("Radius check - " + i);
                RaycastHit hit;
                if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, layerMask)) // If it hits something...
                {
                    // If it's something but not a tile, leave it be
                    if (hit.transform.gameObject.CompareTag("TMTopology") == false)
                    {
                        DebugRay(origin, Vector3.down * hit.distance, Color.blue);
                        continue;
                    }

                    DebugRay(origin, Vector3.down * hit.distance, Color.yellow);
                    Debug.Log("Did Hit " + hit.transform.gameObject.name);
                    _selectableTiles.Add(hit.transform.gameObject);
                    try //Temp: change the material's color. To replace later on
                    {
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 0, 1f);
                    }
                    catch (MissingComponentException e)
                    {
                        Debug.LogError($"An error occured when hitting {hit.transform.gameObject.name}: {e.ToString()}");
                    }
                }
                else // If there's nothing...
                {
                    DebugRay(origin, Vector3.down * 100, Color.red);
                    //Debug.DrawRay(origin, Vector3.down * 1000, Color.red);
                    Debug.Log("Did Hitn't");
                    //break;
                }
            }
        }*/

        int count;
        int count_inner;
        try
        {
            // May look ugly but currently the most efficiant method I have compared to loops in loops in if conditions (22ms vs 66ms on r=7, ir=4)
            int i;
            count_inner = Physics.SphereCastNonAlloc(obj.transform.position, inner_radius, Vector3.down, _innerHitBuffer, inner_radius, layerMask);
            LayerMask[] origLayer = new LayerMask[count_inner];
            for (i = 0; i < count_inner; i++)
            {
                RaycastHit hit = _innerHitBuffer[i];
                origLayer[i] = hit.transform.gameObject.layer;
                hit.transform.gameObject.layer = LayerMask.NameToLayer("TempLayer");
            }
            layerMask &= ~(1 << LayerMask.NameToLayer("TempLayer"));
            count = Physics.SphereCastNonAlloc(obj.transform.position, radius, Vector3.down, _hitBuffer, radius, layerMask);
            for (i = 0; i < count_inner; i++)
            {
                RaycastHit hit = _innerHitBuffer[i];
                hit.transform.gameObject.layer = origLayer[i];
            }
            Debug.Log(count + count_inner);
        }
        catch (Exception e)
        {
            throw new ArgumentOutOfRangeException("[SelectableArea] The numbers of selected tiles is superior to the size of the buffer! (" + e + ")");
        }
        BetterDebug.Log(_hitBuffer.Length, count);

        for (int i = 0; i < count; i++)
        {
            RaycastHit hit = _hitBuffer[i];

            if (hit.transform.gameObject.CompareTag("TMTopology") == false)
            {
                Debug.Log("Did Hit Non-Tile " + hit.transform.gameObject.name);
                continue;
            }
            Debug.Log("Did Hit " + hit.transform.gameObject.name);

            _selectableTiles.Add(hit.transform.gameObject);
            try
            {
                hit.transform.gameObject.GetComponent<Tile>().SetSelected(true);
            }
            catch (MissingComponentException e)
            {
                Debug.LogError($"An error occured when hitting {hit.transform.gameObject.name}: {e}");
            }
        }

        // Clear out the buffer for future use
        Array.Clear(_hitBuffer, 0, count);
        Array.Clear(_innerHitBuffer, 0, count_inner);

        // If there's an actual area set, activate enemies and interactables' raycasting if allowed
        if (_selectableTiles.Count > 0)
        {
            _enemyAreaCheck = _allowSelectEnemy;
            _interactableAreaCheck = _allowSelectInteractable;
        }
        return _selectableTiles;
    }

    /// <summary>
    /// Sets up an area of a defined radius around obj where objects can be selected
    /// </summary>
    /// <param name="obj">The object around which the area would be set</param>
    /// <param name="radius">The radius of the area</param>
    /// <param name="ignore_interactable">Whether the area should include the tiles below interactables or not</param>
    /// <returns>A list of tiles that are part of the selectable area</returns>
    public List<GameObject> FindSelectableArea(GameObject obj, int radius)
    {
        ResetSelectable();

        // Prevents players, interactables (if decided so) and enemies to be counted in the raycast
        int layerMask = 0;
        foreach (var layer in _ignoreLayerList)
        {
            layerMask |= 1 << LayerMask.NameToLayer(layer);
        }
        layerMask = ~layerMask;

        /*for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                // Slighly change the x and z axis litteraly just becaue the player doesn't start in the center of a case currently
                // causing some raycasts to hit the same tile instead of each hitting their own
                Vector3 pos = obj.transform.position + new Vector3(0.25f, 5f, 0.25f);

                Vector3 origin = pos + new Vector3(i * 1f, 0, j * 1f);

                //print("Radius check - " + i);
                RaycastHit hit;
                if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, layerMask)) // If it hits something...
                {
                    // If it's something but not a tile, leave it be
                    if (hit.transform.gameObject.CompareTag("TMTopology") == false)
                    {
                        DebugRay(origin, Vector3.down * hit.distance, Color.blue);
                        continue;
                    }

                    DebugRay(origin, Vector3.down * hit.distance, Color.yellow);
                    //Debug.Log("Did Hit " + hit.transform.gameObject.name);
                    _selectableTiles.Add(hit.transform.gameObject);
                    try
                    {
                        hit.transform.gameObject.GetComponent<Tile>()._selectable = true;
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 0, 1f);
                    }
                    catch (MissingComponentException e)
                    {
                        //Debug.LogError($"An error occured when hitting {hit.transform.gameObject.name}: {e}");
                    }
                }
                else // If there's nothing...
                {
                    DebugRay(origin, Vector3.down * 100, Color.red);
                    //Debug.Log("Did Hitn't");
                    //break;
                }
            }
        }*/

        /*float pointsPerUnitLength = 1.0f;
        float circumference = 2 * Mathf.PI * radius;
        int numberOfPoints = Mathf.CeilToInt(circumference * pointsPerUnitLength);
        float angle_gap = 360f / numberOfPoints;

        for (float angle = 0; angle < 360; angle += angle_gap)
        {
            float rad = Mathf.Deg2Rad * angle;

            float circleX = Mathf.Cos(rad) * radius;
            float circleZ = Mathf.Sin(rad) * radius;

            Vector3 pos = obj.transform.position;
            Vector3 castPoint = new(pos.x + circleX, pos.y + 5f, pos.z + circleZ);

            if (Physics.Raycast(castPoint, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask)) // If it hits something...
            {
                Debug.Log("Did Hit");

                // If it's something but not a tile, leave it be
                if (hit.transform.gameObject.CompareTag("TMTopology") == false)
                {
                    DebugRay(castPoint, Vector3.down * hit.distance, Color.blue);
                    continue;
                }

                DebugRay(castPoint, Vector3.down * hit.distance, Color.yellow);
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
                Debug.Log("Did Hitn't");
                DebugRay(castPoint, Vector3.down * 100, Color.red);
            }
        }

        castAxis(obj.transform.position, layerMask);*/

        int count;
        try
        {
            count = Physics.SphereCastNonAlloc(obj.transform.position, radius, Vector3.down, _hitBuffer, radius, layerMask);
        }
        catch (Exception e)
        {
            throw new ArgumentOutOfRangeException("[SelectableArea] The numbers of selected tiles is superior to the size of the buffer! (" + e + ")");
        }
        BetterDebug.Log(_hitBuffer.Length, count);

        for (int i = 0; i < count; i++)
        {
            RaycastHit hit = _hitBuffer[i];

            if (hit.transform.gameObject.CompareTag("TMTopology") == false)
            {
                Debug.Log("Did Hit Non-Tile " + hit.transform.gameObject.name);
                continue;
            }
            //Debug.Log("Did Hit " + hit.transform.gameObject.name);

            _selectableTiles.Add(hit.transform.gameObject);
            try
            {
                hit.transform.gameObject.GetComponent<Tile>().SetSelected(true);
            }
            catch (MissingComponentException e)
            {
                Debug.LogError($"An error occured when hitting {hit.transform.gameObject.name}: {e}");
            }
        }

        // Clear out the buffer for future use
        Array.Clear(_hitBuffer, 0, count);

        // If there's an actual area set, activate enemies and interactables' raycasting if allowed
        if (_selectableTiles.Count > 0)
        {
            _enemyAreaCheck = _allowSelectEnemy;
            _interactableAreaCheck = _allowSelectInteractable;
        }
        return _selectableTiles;
    }

    /*void castAxis(Vector3 pos, int mask=0)
    {
        pos.y += 5f;
        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, Mathf.Infinity, mask))
        {

        }
    }*/

    public List<GameObject> GetSelectableTiles()
    {
        return _selectableTiles;
    }

    /// <summary>
    /// Removes the currently set selectable area, reset the color of the tiles (temp) and deactivates enemies and interactables raycasting
    /// </summary>
    public void ResetSelectable()
    {
        ResetDebugRay();

        foreach (var tile in _selectableTiles)
        {
            tile.GetComponent<Tile>().SetSelected(false);
        }
        _selectableTiles.Clear();

        SetGroundColor(Color.white);

        _enemyAreaCheck = false;
        _interactableAreaCheck = false;
    }

    /// <summary>
    /// Sets up which entities should be allowed to be selected when inside the area
    /// </summary>
    /// <param name="allowPlayer">True if the player can be selected. False otherwise. Defaults to false</param>
    /// <param name="allowInteractables">True if interactables can be selected. False otherwise. Defaults to false</param>
    /// <param name="allowEnemies">True if enemies can be selected. False otherwise. Defaults to true</param>
    /// <param name="allowTiles">True if a tile can be selected. False otherwise. Defaults to false</param>
    public void SetSelectableEntites(bool allowPlayer = false, bool allowInteractables = false, bool allowEnemies = true, bool allowTiles = false)
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

    /// <summary>
    /// Checks if the mouse is hovering a object that can be selected. True if so, false otherwise.
    /// </summary>
    /// <param name="obj">The object the mouse selected</param>
    /// <param name="removeSelectable">If true, the area will be removed if an object is returned</param>
    /// <returns></returns>
    public bool CastLeftClick(out GameObject obj, bool removeSelectable = true)
    {
        obj = null;

        // Filter out any entities' layer that isn't allowed to be selected
        int layerMask = 0;
        foreach (var layer in _ignoreLayerList)
        {
            if (layer == "Player" && _allowSelectPlayer) continue;
            if (layer == "Interactable" && _allowSelectInteractable) continue;
            if (layer == "Enemy" && _allowSelectEnemy) continue;
            layerMask |= (1 << LayerMask.NameToLayer(layer));
        }
        layerMask = ~layerMask;
        // Makes sure the default layer is included in the raycast for tiles
        layerMask |= (1 << LayerMask.NameToLayer("Default"));

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, layerMask) == false) return false;

        GameObject objHit = hit.transform.gameObject;

        // If the object hit by the raycast is one specific type of object we're looking for, set it as the out object
        if (
            (objHit.TryGetComponent(out Tile tile) && tile.IsSelectable && _allowSelectTiles) ||
            (objHit.TryGetComponent(out Enemy enemy) && enemy.IsSelectable) ||
            (objHit.TryGetComponent(out Interactible interact) && interact.IsSelectable)
            )
        {
            obj = objHit;
        }
        // Removes the area if an object has been found
        if (removeSelectable && obj != null)
            ResetSelectable();
        return obj != null;
    }

    public bool CheckForSelectableTile(Vector3 pos, int y_offset=5)
    {
        pos.y += y_offset;

        // Filter out any entities' layer that isn't allowed to be selected
        int layerMask = 0;
        foreach (var layer in _ignoreLayerList)
        {
            layerMask |= (1 << LayerMask.NameToLayer(layer));
        }
        layerMask = ~layerMask;

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask) == false) return false;
        print("Raycast pass");

        if (hit.transform.gameObject.TryGetComponent(out Tile tile) == false) return false;
        print("Tile pass and it's "+tile.IsSelectable);

        return tile.IsSelectable;
    }

    // Temporary: to replace with an overlay or something similar
    public void SetGroundColor(Color color)
    {
        //print("Yeh");
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
