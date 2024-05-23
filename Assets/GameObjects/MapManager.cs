using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MapManager : MonoBehaviour
{
    int _mapSizeX;
    int _mapSizeY;
    List<List<MapNode>> _mapGrid;
    List<Vector2> _startingCoords;

    GameObject _mapNodePrefab;

    class MapNode
    {
        public List<Vector2> _prevRooms;
        public GameObject _mapNodeObject;

        public MapNode()
        {
            _prevRooms = new List<Vector2>();
        }

        public MapNode(List<Vector2> prevRoom)
        {
            _prevRooms = prevRoom;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _mapSizeX = 7;
        _mapSizeY = 15;

        _mapNodePrefab = (GameObject)Resources.Load("Map Node");
    }

    void InitMapGrid()
    {
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
                _mapGrid[i][j]._mapNodeObject = obj;
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
        Vector2 prevRoom;
        for (int pathNb = 0; pathNb < 4; pathNb++)
        {
            print($"Path : {pathNb}");
            int newStartCoordIndex = Random.Range(0, _startingCoords.Count);
            prevRoom = new Vector2(_startingCoords[newStartCoordIndex].x, 0);
            _startingCoords.RemoveAt(newStartCoordIndex);
            foreach (Vector2 item in _startingCoords)
            {
                print($"Starting Coord: {item}");
            }
            //print($"Sarting coords : {_startingCoords.Count}");
            for (int floorNb = 0; floorNb < _mapSizeY; floorNb++)
            {
                int targetRoom = (int)prevRoom.x;
                int selectedRoom = Mathf.Clamp(Random.Range(targetRoom-1, targetRoom+2), 0, _mapSizeX-1);
                //print($"Selected Room : {selectedRoom}");
                MapNode curNode = _mapGrid[selectedRoom][floorNb];
                curNode._prevRooms.Add(prevRoom);
                curNode._mapNodeObject.gameObject.GetComponent<LineRenderer>().SetPosition(0, curNode._mapNodeObject.gameObject.transform.position);
                curNode._mapNodeObject.gameObject.GetComponent<LineRenderer>().SetPosition(1, _mapGrid[(int)curNode._prevRooms[0].x][(int)curNode._prevRooms[0].y]._mapNodeObject.gameObject.transform.position);

                prevRoom = new Vector2(selectedRoom, floorNb);
            }
        }

        // Cleaning undesired paths
        for (int i = 0; i < _mapSizeX; i++)
        {
            for (int j = 0; j < _mapSizeY; j++)
            {
                if (_mapGrid[i][j]._prevRooms.Count == 0)
                {
                    _mapGrid[i][j]._mapNodeObject.gameObject.SetActive(false);
                }
            }
        }
    }

    void DisplayMap()
    {

        for (int i = 0; i < _mapSizeX; i++)
        {
            for (int j = 0; j < _mapSizeY; j++)
            {

            }
        }
    }

    bool ArePathsCrossing(MapNode room1, MapNode room2)
    {

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateMap();
            DisplayMap();
        }
    }
}
