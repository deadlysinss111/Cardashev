using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class MapManager : MonoBehaviour
{
    int _mapSizeX;
    int _mapSizeY;

    // For map generation
    List<GameObject> _startingNodes;
    int NUMBER_OF_PATH;

    // PREFABS
    GameObject MAP_NODE;
    GameObject MAP_PATH;
    
    // For map navigation
    List<List<GameObject>> _mapGrid;
    GameObject _playerLocation; // TODO auto create this one and make it invisible at Start()
    GameObject _bossRoom;

    // Miscenalious
    [SerializeField] LayerMask _clickableLayers;

    void Start()
    {
        NUMBER_OF_PATH = 4;
        _mapGrid = GlobalInformations._mapNodes;

        if (_mapGrid != null)
        {
            return;
        }

        _mapSizeX = 7;
        _mapSizeY = 8;

        _mapGrid = new List<List<GameObject>>(_mapSizeX);
        _startingNodes = new List<GameObject>();

        MAP_NODE = (GameObject)Resources.Load("Map Node");
        MAP_PATH = (GameObject)Resources.Load("Map Path");
        // Generate an invisible starting node
        _playerLocation = Instantiate(MAP_NODE).transform.gameObject;
        _playerLocation.GetComponent<MapNode>()._nextNodes = new GameObject[NUMBER_OF_PATH];
        _playerLocation.GetComponent<MapNode>().SetAsOriginalNode();

        GenerateMap();
        GlobalInformations._mapNodes = _mapGrid;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastTarget();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateMap();
        }
    }

    void MovePlayerTo(GameObject nodeToMoveTo)
    {
        nodeToMoveTo.GetComponent<MapNode>().SelectNode();
        _playerLocation.GetComponent<MapNode>().UnselectNode();
        _playerLocation = nodeToMoveTo;
    }

    void RaycastTarget()
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

    void InitMapGrid()
    {
        Transform mapObj = GameObject.FindGameObjectsWithTag("Map Node Parent")[0].transform;

        int spaceBetweenNodes = 5;

        for (int i = 0; i < _mapSizeX; i++)
        {
            _mapGrid.Add(new List<GameObject>(_mapSizeY));
            for (int j = 0; j < _mapSizeY; j++)
            {
                GameObject obj = Instantiate(MAP_NODE);
                _mapGrid[i].Add(obj);
                obj.GetComponent<MapNode>()._nextNodes = new GameObject[NUMBER_OF_PATH];
                obj.transform.SetParent(mapObj, false);

                obj.transform.localPosition = new Vector3(i * spaceBetweenNodes, 4, j * spaceBetweenNodes);
            }
        }

        _bossRoom = Instantiate(MAP_NODE);
        _bossRoom.transform.SetParent(mapObj, false);
        _bossRoom.transform.localPosition = new Vector3(_mapSizeX/2 * spaceBetweenNodes, 4, (_mapSizeY+1) * spaceBetweenNodes);
        _bossRoom.transform.localScale *= 2;
        _bossRoom.GetComponent<MapNode>().SetRoomTypeTo(RoomType.Boss);
    }

    void GenerateMap()
    {
        InitMapGrid();

        // WIP Path generation

        // Starting nodes attribution
        List<GameObject> freeNodelist = new List<GameObject>();
        for (int i = 1; i < _mapSizeX-1; i++)
        {
            _mapGrid[i][0].GetComponent<MapNode>()._startingXCoord = i;
            freeNodelist.Add(_mapGrid[i][0]);
        }
        for (int y = 0; y < NUMBER_OF_PATH; y++)
        {
            int newStartCoordIndex = Random.Range(0, freeNodelist.Count);
            GameObject startingNode = freeNodelist[newStartCoordIndex];
            startingNode.GetComponent<MapNode>()._isStartingNode = true;
            startingNode.name = "Starting Node";
            _playerLocation.GetComponent<MapNode>()._nextNodes[y] = startingNode;
            _startingNodes.Add(startingNode);
            freeNodelist.RemoveAt(newStartCoordIndex);
        }

        // Flag just for putting a "Rest" room at the end of the first path
        //bool isFirstPath = true;
        // Generating paths between each node
        for (int nodeNb=0; nodeNb < _startingNodes.Count; nodeNb++)
        {
            int x = _startingNodes[nodeNb].GetComponent<MapNode>()._startingXCoord;
            for (int floorNb = 0; floorNb < _mapSizeY-1; floorNb++)
            {
                int nextNodeXIndex;
                if (floorNb == 0)
                {
                    nextNodeXIndex = x;
                }
                else
                {
                    nextNodeXIndex = Random.Range(Mathf.Clamp(x - 1, 0, _mapSizeX - 1), Mathf.Clamp(x + 2, 0, _mapSizeX - 1));
                }
                nextNodeXIndex = Mathf.Clamp(ArePathsCrossing(x, floorNb, nextNodeXIndex), 0, _mapSizeX - 1);

                GameObject nextNode = _mapGrid[nextNodeXIndex][floorNb + 1];
                _mapGrid[x][floorNb].GetComponent<MapNode>()._nextNodes[nodeNb] = nextNode;

                // WIP placing paths between nodes
                CreatePathBetween(_mapGrid[x][floorNb], nextNode);

                // Saving the x coordinate of the next node for the next iteration of the loop
                x = nextNodeXIndex;
                if (floorNb == _mapSizeY - 2)
                {
                    nextNode.GetComponent<MapNode>()._nextNodes[nodeNb] = _bossRoom;
                    CreatePathBetween(nextNode, _bossRoom);
                }
            }
            //isFirstPath = false;
        }

        // Disabling path-less nodes to make them invisible
        for (int i = 0; i < _mapSizeX; i++)
        {
            for (int j = 0; j < _mapSizeY; j++)
            {
                _mapGrid[i][j].SetActive(_mapGrid[i][j].GetComponent<MapNode>().HasNextNode());
            }
        }

        GiveTypeToRooms();
    }

    void CreatePathBetween(GameObject node1, GameObject node2)
    {
        GameObject newPath = Instantiate(MAP_PATH);
        newPath.transform.SetParent(GameObject.FindGameObjectWithTag("Map Path Parent").transform, false);
        newPath.GetComponent<MapPathScript>().SetPathPoints(node1, node2);
    }

    void GiveTypeToRooms()
    {
        // Elite: 1-2/Zone
        int eliteToPlace = (Random.Range(0, 100) >= 50 ? 1 : 2);
        //print(eliteToPlace);
        
        for (int pathNb = 0; pathNb < _startingNodes.Count; pathNb++)
        {
            bool needToPlaceElite = true;
            //print("======================== Next Path ========================");
            // Stores every rooms in this path, which will then be randomly placed along the path.
            List<RoomType> rooms = new List<RoomType>();
            // Rules: every path start with a Fight, at least 1 path has a Rest right before the Boss, then fill with the following ratios:
            // Shop: 1/path
            int shopToPlace = 1;
            // Rest: 75% -> 1/path + 25% -> 1/path
            int restToPlace = 1;// + (Random.Range(0, 100) >= 25 ? 0 : 1);
            // remaining nodes are 50/50 Fights or Events
            int roomsToPlace = 0;
            // Current percentage to place a Fight room
            int fightPerc = 50;
            MapNode curRoom = _startingNodes[pathNb].GetComponent<MapNode>()._nextNodes[pathNb].GetComponent<MapNode>();
            while (curRoom.RoomType != RoomType.Boss)
            {
                switch (curRoom.RoomType)
                {
                    case RoomType.Shop:
                        shopToPlace--;
                        break;
                    case RoomType.Rest:
                        restToPlace--;
                        break;
                    case RoomType.Elite:
                        roomsToPlace--;
                        break;
                    case RoomType.Combat:
                        roomsToPlace--;
                        break;
                    case RoomType.Event:
                        roomsToPlace--;
                        break;
                }
                curRoom = curRoom._nextNodes[pathNb].GetComponent<MapNode>();
                roomsToPlace++;
            }
            /*print("roomsToPlace: " + roomsToPlace);
            print("shopToPlace: " + shopToPlace);
            print("restToPlace: " + restToPlace);
            print("eliteToPlace: " + eliteToPlace);*/
            int fightEventToPlace = roomsToPlace - Mathf.Clamp(shopToPlace, 0, 1) - Mathf.Clamp(restToPlace, 0, 1);

            while (shopToPlace > 0)
            {
                rooms.Insert(Random.Range(0, rooms.Count + 1), RoomType.Shop);
                shopToPlace--;
            }
            while (restToPlace > 0)
            {
                rooms.Insert(Random.Range(0, rooms.Count + 1), RoomType.Rest);
                restToPlace--;
            }
            while (fightEventToPlace > 0)
            {
                if (Random.Range(0, 100) <= fightPerc)
                {
                    if (needToPlaceElite && eliteToPlace > 0)
                    {
                        rooms.Insert(Random.Range(0, rooms.Count + 1), RoomType.Elite);
                        eliteToPlace--;
                        needToPlaceElite = false;
                    }
                    else
                    {
                        rooms.Insert(Random.Range(0, rooms.Count + 1), RoomType.Combat);
                        fightPerc -= 15;
                    }
                }
                else
                {
                    rooms.Insert(Random.Range(0, rooms.Count + 1), RoomType.Event);
                    fightPerc += 15;
                }
                fightEventToPlace--;
            }

            /*foreach (var item in rooms)
            {
                print("Room type: " + item);
            }*/

            // Place a Fight on the first room
            _startingNodes[pathNb].GetComponent<MapNode>().SetRoomTypeTo(RoomType.Combat);
            curRoom = _startingNodes[pathNb].GetComponent<MapNode>()._nextNodes[pathNb].GetComponent<MapNode>();

            while (curRoom.RoomType != RoomType.Boss)
            {
                if (curRoom.RoomType == RoomType.None)
                {
                    curRoom.SetRoomTypeTo(rooms[0]);
                    rooms.RemoveAt(0);
                }
                // WARNING TODO : CHECK
                curRoom = curRoom._nextNodes[Mathf.Clamp(pathNb, 0, curRoom._nextNodes.Length - 1)].GetComponent<MapNode>();
            }
            //print("Remaining rooms: "+rooms.Count);
        }
    }

    int ArePathsCrossing(int x, int floor, int nextX)
    {
        if (x == nextX) return nextX;

        List<int> correctedX = new List<int> { x };

        foreach (GameObject node in _mapGrid[nextX][floor].GetComponent<MapNode>()._nextNodes)
        {
            if (ReferenceEquals(node, _mapGrid[x][floor+1]))
            {
                correctedX.Add(nextX + 2 * (x < nextX ? -1 : 1));
                if (Mathf.Round(correctedX[1] - x) > 1)
                {
                    return x;
                }
            }
        }

        if (correctedX.Count == 1)
        {
            return nextX;
        }
        return correctedX[Random.Range(0, 2)];
    }
}
