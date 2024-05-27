using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

/*public class MapNode
{
    public List<GameObject> _nextRooms;
    public GameObject _mapNodeObject;

    public bool _isStartRoom;

    public MapNode()
    {
        _nextRooms = new List<GameObject>();
        _isStartRoom = false;
    }
}*/

public class MapManager : MonoBehaviour
{
    int _mapSizeX;
    int _mapSizeY;

    // For map generation
    List<GameObject> _startingNodes;

    // PREFABS
    GameObject _mapNodePrefab;
    GameObject _mapPathPrefab;
    
    // For map navigation
    [SerializeField] GameObject _startingNode; // TODO auto create this one and make it invisible at Start()
    List<List<GameObject>> _mapGrid;
    GameObject _playerLocation;

    // Miscenalious
    [SerializeField] LayerMask _clickableLayers;

    void Start()
    {
        _mapSizeX = 7;
        _mapSizeY = 15;

        _mapGrid = new List<List<GameObject>>(_mapSizeX);

        _mapNodePrefab = (GameObject)Resources.Load("Map Node");
        _mapPathPrefab = (GameObject)Resources.Load("Map Path");
        // Generate an invisible starting node
        _playerLocation = Instantiate(_mapNodePrefab).transform.gameObject;
        _playerLocation.transform.SetParent(GameObject.FindGameObjectsWithTag("Map")[0].transform, false);
        _playerLocation.transform.localPosition = new Vector3(1, 3);
        _playerLocation.GetComponent<MapNode>().SelectNode();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            // Use a Raycast to get the map node that was targeted
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, _clickableLayers))
            {
                // Get the MapNode Component
                GameObject curNode = hit.transform.gameObject;
                foreach (GameObject nextNode in _playerLocation.GetComponent<MapNode>()._nextNodes)
                {
                    if (ReferenceEquals(curNode, nextNode))
                    {
                        MovePlayerTo(curNode);
                        break;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateMap();
        }
    }

    void MovePlayerTo(GameObject nodeToMoveTo)
    {
        nodeToMoveTo.GetComponent<MapNode>().SelectNode();
        _playerLocation = nodeToMoveTo;
        _playerLocation.GetComponent<MapNode>().UnselectNode();
    }

    void InitMapGrid()
    {
        // Destroy all children if they already exist
        Transform mapObj = GameObject.FindGameObjectsWithTag("Map Node Parent")[0].transform;
        foreach(Transform child in mapObj.transform)
        {
            Destroy(child.gameObject);
        }

        int spaceBetweenNodes = 5;

        _startingNodes = new List<GameObject>();

        for (int i = 0; i < _mapSizeX; i++)
        {
            _mapGrid.Add(new List<GameObject>(_mapSizeY));
            for (int j = 0; j < _mapSizeY; j++)
            {
                GameObject obj = Instantiate(_mapNodePrefab);
                _mapGrid[i].Add(obj);
                obj.transform.SetParent(mapObj, false);
                obj.transform.localPosition = new Vector3(i * spaceBetweenNodes, 4, j * spaceBetweenNodes);

            }
        }

        // Generate starting coords
        for (int i = 0; i < _mapSizeX; i++)
        {
            _startingNodes.Add(_mapGrid[i][0]);
        }
    }

    void GenerateMap()
    {
        InitMapGrid();

        // WIP Path generation
        /*GameObject nextRoom;
        for (int pathNb = 0; pathNb < 4; pathNb++)
        {
            int newStartCoordIndex = Random.Range(0, _startingNodes.Count);
            nextRoom = new Vector2(_startingNodes[newStartCoordIndex], 0);
            _startingNodes.RemoveAt(newStartCoordIndex);
            //print($"Sarting coords : {_startingNodes.Count}");
            for (int floorNb = 1; floorNb < _mapSizeY; floorNb++)
            {
                int targetRoom = (int)nextRoom.x;
                int selectedRoom = Mathf.Clamp(Random.Range(targetRoom - 1, targetRoom + 2), 0, _mapSizeX - 1);
                //print($"Selected Room : {selectedRoom}");
                MapNode curNode = _mapGrid[selectedRoom][floorNb];
                curNode._nextRooms.Add(nextRoom);
                //curNode._mapNodeObject.gameObject.GetComponent<LineRenderer>().SetPosition(0, curNode._mapNodeObject.gameObject.transform.position);
                //curNode._mapNodeObject.gameObject.GetComponent<LineRenderer>().SetPosition(1, _mapGrid[(int)curNode._prevRooms[0].x][(int)curNode._prevRooms[0].y]._mapNodeObject.gameObject.transform.position);
                nextRoom = new Vector2(selectedRoom, floorNb);
            }
        }*/

        for (int y = 0; y < 4; y++)
        {
            print($"{}");
            int newStartCoordIndex = Random.Range(0, _startingNodes.Count);
            _startingNodes[newStartCoordIndex].GetComponent<MapNode>()._isStartRoom = true;
            _startingNodes.RemoveAt(newStartCoordIndex);
        }

        // Cleaning undesired nodes
        for (int i = 0; i < _mapSizeX; i++)
        {
            for (int j = 0; j < _mapSizeY; j++)
            {
                if (!_mapGrid[i][j].GetComponent<MapNode>()._isStartRoom)
                {
                    _mapGrid[i][j].SetActive(false);
                }
            }
        }
    }

    bool ArePathsCrossing(MapNode room1, MapNode room2)
    {

        return false;
    }
}
