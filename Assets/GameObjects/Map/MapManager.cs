using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
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
    List<List<MapNode>> _mapGrid;
    List<Vector2> _startingCoords;

    GameObject _mapNodePrefab;
    GameObject _mapPathPrefab;
    [SerializeField] GameObject _startingNode;
    MapNode _playerLocation;
    [SerializeField] LayerMask _clickableLayers;

    void Start()
    {
        _mapSizeX = 7;
        _mapSizeY = 15;

        _mapNodePrefab = (GameObject)Resources.Load("Map Node");
        _mapPathPrefab = (GameObject)Resources.Load("Map Path");
        _playerLocation = GameObject.FindGameObjectsWithTag("Map Starting Node")[0].transform.GetComponent<MapNode>();
        _playerLocation.SelectNode();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            // Uses a Raycast to get the map node that was targeted
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, _clickableLayers))
            {
                // Get the MapNode Component
                MapNode node = hit.transform.gameObject.GetComponent<MapNode>();
                for (int i = 0; i < _playerLocation._nextNodes.Count; i++)
                {
                    if(ReferenceEquals(node, _playerLocation._nextNodes[i]))
                    {
                        node.SelectNode();
                        _playerLocation = node;
                        _playerLocation.UnselectNode();
                        break;
                    }
                }
            }
        }

        /*if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateMap();
        }*/
    }

    void MovePlayerOnMap()
    {

    }

    void InitMapGrid()
    {
        // Destroy all children if they already exist
        Transform mapCanvasObj = GameObject.FindGameObjectsWithTag("Map Canvas")[0].transform;
        int childrenToRemove = mapCanvasObj.childCount;
        for (int i = 0; i < childrenToRemove; i++)
        {
            Destroy(mapCanvasObj.GetChild(i).gameObject);
        }

        int spaceBetweenNodes = 50;

        _mapGrid = new List<List<MapNode>>();
        _startingCoords = new List<Vector2>();

        for (int i = 0; i < _mapSizeX; i++)
        {
            _startingCoords.Add(new Vector2(i, 0));
            _mapGrid.Add(new List<MapNode>());
            for (int j = 0; j < _mapSizeY; j++)
            {
                _mapGrid[i].Add(new MapNode());
                GameObject obj = Instantiate(_mapNodePrefab);
                _mapGrid[i][j] = obj.GetComponent<MapNode>();
                //// WARNING : don't forget to create a "Map Canvas" tag when merging codes !!
                obj.transform.SetParent(GameObject.FindGameObjectsWithTag("Map Canvas")[0].transform, false);
                obj.transform.localPosition = new Vector3(i * spaceBetweenNodes, j * spaceBetweenNodes);

            }
        }
        /*print($"size x : {_mapGrid.Count}");
        print($"size y : {_mapGrid[0].Count}");
        print($"Sarting coords : {_startingCoords.Count}");*/
    }

    void GenerateMap()
    {
        InitMapGrid();

        // WIP Path generation
        /*GameObject nextRoom;
        for (int pathNb = 0; pathNb < 4; pathNb++)
        {
            print($"Path : {pathNb}");
            int newStartCoordIndex = Random.Range(0, _startingCoords.Count);
            nextRoom = new Vector2(_startingCoords[newStartCoordIndex].x, 0);
            _startingCoords.RemoveAt(newStartCoordIndex);
            foreach (Vector2 item in _startingCoords)
            {
                print($"Starting Coord: {item}");
            }
            //print($"Sarting coords : {_startingCoords.Count}");
            for (int floorNb = 1; floorNb < _mapSizeY; floorNb++)
            {
                int targetRoom = (int)nextRoom.x;
                int selectedRoom = Mathf.Clamp(Random.Range(targetRoom-1, targetRoom+2), 0, _mapSizeX-1);
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
            int newStartCoordIndex = Random.Range(0, _startingCoords.Count);
            print(_startingCoords[newStartCoordIndex].y);
            _mapGrid[(int)_startingCoords[newStartCoordIndex].x][0]._isStartRoom = true;
            _startingCoords.RemoveAt(newStartCoordIndex);
        }

        // Cleaning undesired nodes
        for (int i = 0; i < _mapSizeX; i++)
        {
            for (int j = 0; j < _mapSizeY; j++)
            {
                if (!_mapGrid[i][j]._isStartRoom)
                {
                    _mapGrid[i][j].gameObject.SetActive(false);
                }
            }
        }
    }

    bool ArePathsCrossing(MapNode room1, MapNode room2)
    {

        return false;
    }
}
