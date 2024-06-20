using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    // For map generation
    List<GameObject> _startingNodes;
    [SerializeField] int MAP_SIZE_X = 8; // Number of columns
    [SerializeField] int MAP_SIZE_Y = 10; // Number of floors
    [SerializeField] int NUMBER_OF_PATH = 6; // It's in the name dumbass

    // PREFABS
    GameObject MAP_NODE;
    GameObject MAP_PATH;
    GameObject BLOCKER;
    GameObject RADIOACTIVE_CLOUD;
    [SerializeField] GameObject _radioactiveCloud;

    // Resources
    [NonSerialized] public MapResourceLoader resources;

    // For map navigation
    List<List<GameObject>> _mapGrid;
    GameObject _playerLocation; // TODO auto create this one and make it invisible at Start()
    GameObject _bossRoom;
    CameraManager _cameraManager;

    // Miscenalious
    [SerializeField] LayerMask _clickableLayers;
    [SerializeField] float _BlockerProbability;

    private void Awake()
    {
        resources = GetComponent<MapResourceLoader>();
        _cameraManager = GetComponent<CameraManager>();
    }

    void Start()
    {
        _mapGrid = GI._mapNodes;

        if (_mapGrid != null)
        {
            return;
        }

        _mapGrid = new List<List<GameObject>>(MAP_SIZE_X);
        _startingNodes = new List<GameObject>();

        MAP_NODE = (GameObject)Resources.Load("Map Node");
        MAP_PATH = (GameObject)Resources.Load("Map Path");
        BLOCKER = (GameObject)Resources.Load("TimerDoor");
        RADIOACTIVE_CLOUD = (GameObject)Resources.Load("Radioactive Cloud");
        // Generate an invisible starting node
        _playerLocation = Instantiate(MAP_NODE);
        _playerLocation.GetComponent<MapNode>()._nextNodes = new GameObject[NUMBER_OF_PATH];
        _playerLocation.GetComponent<MapNode>().SetAsOriginalNode();
        _playerLocation.GetComponent<MapNode>().SelectNode();
        GenerateMap();
        GI._mapNodes = _mapGrid;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastTarget();
        }

        if (Input.GetMouseButtonDown(1))
        {
            MoveCloud();
        }
    }

    void MovePlayerTo(GameObject nodeToMoveTo)
    {
        nodeToMoveTo.GetComponent<MapNode>().SelectNode();
        _playerLocation.GetComponent<MapNode>().UnselectNode();
        _playerLocation = nodeToMoveTo;
        _cameraManager.UpdateNodeTarget(nodeToMoveTo);
    }

    void LockAllNodes()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Map Node");
        foreach (GameObject node in nodes)
        {
            if (ReferenceEquals(node, _bossRoom)) continue;
            node.GetComponent<MapNode>().LockNode();
        }
    }

    void RecursiveUnlock(GameObject node, bool firstCall)
    {
        MapNode curNode = node.GetComponent<MapNode>();
        if (ReferenceEquals(curNode, _bossRoom)) return;
        if (!firstCall) curNode.UnlockNode();
        foreach (GameObject nextNode in curNode._nextNodes)
        {
            if (!nextNode || nextNode.GetComponent<MapNode>().IsLockedByBlocker()) continue;
            RecursiveUnlock(nextNode, false);
        }
    }

    void RaycastTarget()
    {
        RaycastHit hit;
        // Use a Raycast to get the map node that was targeted
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, _clickableLayers))
        {
            // Get the MapNode Component
            GameObject targetNode = hit.transform.gameObject;
            foreach (GameObject nextNode in _playerLocation.GetComponent<MapNode>()._nextNodes)
            {
                if (ReferenceEquals(targetNode, nextNode))
                {
                    if (targetNode.GetComponent<MapNode>().IsLockedByBlocker()) return;
                    MovePlayerTo(targetNode);
                    LockAllNodes();
                    RecursiveUnlock(_playerLocation, true);
                    break;
                }
            }
        }
    }

    void MoveCloud()
    {
        RaycastHit hit;
        // Use a Raycast to get the map node that was targeted
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, _clickableLayers))
        {
            _radioactiveCloud.GetComponent<ToxicTornado>().UpdTarget(hit.transform.position);
        }
    }

    void InitMapGrid()
    {
        Transform mapObj = GameObject.FindGameObjectsWithTag("Map Node Parent")[0].transform;

        int spaceBetweenNodes = 8;

        for (int i = 0; i < MAP_SIZE_X; i++)
        {
            _mapGrid.Add(new List<GameObject>(MAP_SIZE_Y));
            for (int j = 0; j < MAP_SIZE_Y; j++)
            {
                GameObject obj = Instantiate(MAP_NODE);
                _mapGrid[i].Add(obj);
                obj.GetComponent<MapNode>()._nextNodes = new GameObject[NUMBER_OF_PATH];
                obj.transform.SetParent(mapObj, false);
                obj.transform.localPosition = new Vector3(i * spaceBetweenNodes, 4, j * spaceBetweenNodes);
                float moveAmplitude = 3f;
                int rotAmplitude = 45;
                obj.transform.position += new Vector3(Random.Range(-moveAmplitude, moveAmplitude + .1f), 0, 0);
                obj.transform.rotation = Quaternion.AngleAxis(Random.Range(-rotAmplitude, rotAmplitude + 1), Vector3.up);
            }
        }

        _bossRoom = Instantiate(MAP_NODE);
        _bossRoom.transform.SetParent(mapObj, false);
        _bossRoom.transform.localPosition = new Vector3(MAP_SIZE_X / 2 * spaceBetweenNodes, 4, (MAP_SIZE_Y + 1) * spaceBetweenNodes);
        _bossRoom.transform.position = new Vector3(8, 52, 155);
        _bossRoom.transform.localScale *= 2;
        _bossRoom.GetComponent<MapNode>().SetRoomTypeTo(RoomType.Boss, resources);
    }

    void GenerateMap()
    {
        InitMapGrid();

        // WIP Path generation

        // Starting nodes attribution
        List<GameObject> freeNodelist = new List<GameObject>();
        for (int i = 1; i < MAP_SIZE_X - 1; i++)
        {
            _mapGrid[i][0].GetComponent<MapNode>()._startingXCoord = i;
            freeNodelist.Add(_mapGrid[i][0]);
        }
        for (int pathNb = 0; pathNb < NUMBER_OF_PATH; pathNb++)
        {
            int newStartCoordIndex = Random.Range(0, freeNodelist.Count);
            GameObject startingNode = freeNodelist[newStartCoordIndex];
            startingNode.GetComponent<MapNode>()._isStartingNode = true;
            startingNode.name = "Starting Node";
            _playerLocation.GetComponent<MapNode>()._nextNodes[pathNb] = startingNode;
            startingNode.GetComponent<MapNode>().IsSelectable(true);
            _startingNodes.Add(startingNode);
            freeNodelist.RemoveAt(newStartCoordIndex);
        }

        // Generating paths between each node

        int blockerToPlace = 2;
        for (int nodeNb = 0; nodeNb < _startingNodes.Count; nodeNb++)
        {
            // Flag for placing only 1 blocker per path
            bool blockerWasPlaced = false;
            int x = _startingNodes[nodeNb].GetComponent<MapNode>()._startingXCoord;
            for (int floorNb = 0; floorNb < MAP_SIZE_Y - 1; floorNb++)
            {
                int nextNodeXIndex;
                if (floorNb == 0)
                {
                    nextNodeXIndex = x;
                }
                else
                {
                    List<int> availableX = new List<int>() { x };
                    if (AvailableDirection(x, floorNb, x + 1, nodeNb))
                    {
                        availableX.Add(x + 1);
                    }
                    if (AvailableDirection(x, floorNb, x - 1, nodeNb))
                    {
                        availableX.Add(x - 1);
                    }
                    nextNodeXIndex = availableX[Random.Range(0, availableX.Count)];
                }
                GameObject nextNode = _mapGrid[nextNodeXIndex][floorNb + 1];

                _mapGrid[x][floorNb].GetComponent<MapNode>().AddNextNode(nodeNb, nextNode);
                Material currentMat = _mapGrid[x][floorNb].GetComponent<MapNode>()._RoomIcon3D.GetComponent<MeshRenderer>().materials[0];
                currentMat.SetVector("_General_Offset", new Vector4(x, floorNb));
                currentMat.SetFloat("_Hologram_Density", 16);

                // WIP placing paths between nodes
                GameObject newPath = CreatePathBetween(_mapGrid[x][floorNb], nextNode);
                if (_mapGrid[x][floorNb].GetComponent<MapNode>().UniqueNextNode >= 2 && blockerToPlace > 0 && !blockerWasPlaced)
                {
                    blockerToPlace--;
                    blockerWasPlaced = true;
                    //nextNode.GetComponent<MapNode>()._blocker = GenerateBlocker(newPath).GetComponent<MapBlocker>();
                }

                // Saving the x coordinate of the next node for the next iteration of the loop
                x = nextNodeXIndex;
                if (floorNb == MAP_SIZE_Y - 2)
                {
                    nextNode.GetComponent<MapNode>().AddNextNode(nodeNb, _bossRoom);
                    CreatePathBetween(nextNode, _bossRoom);
                }
            }
        }

        // Destroying path-less nodes
        for (int i = 0; i < MAP_SIZE_X; i++)
        {
            for (int j = 0; j < MAP_SIZE_Y; j++)
            {
                if (_mapGrid[i][j].GetComponent<MapNode>().UniqueNextNode == 0)
                    Destroy(_mapGrid[i][j]);
            }
        }

        // Setting the position of the camera to the "center".
        Vector3 centerPos = new Vector3(0, 0, 0);
        foreach (GameObject node in _startingNodes)
        {
            centerPos += node.transform.position;
        }
        centerPos = centerPos / _startingNodes.Count;

        _playerLocation.transform.position = centerPos;
        _cameraManager.SetCamPos(centerPos, true);

        GiveTypeToRooms();
    }

    GameObject CreatePathBetween(GameObject node1, GameObject node2)
    {
        GameObject newPath = Instantiate(MAP_PATH);
        newPath.transform.SetParent(GameObject.FindGameObjectWithTag("Map Path Parent").transform, false);

        newPath.GetComponent<MeshRenderer>().material.color = Color.gray;
        Vector3 direction = node2.transform.position - node1.transform.position;

        newPath.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        float newZ = Vector3.Distance(node1.transform.position, node2.transform.position);
        newPath.transform.localScale = new Vector3(.3f, .3f, newZ);

        Vector3 pos1 = node1.transform.position;
        Vector3 pos2 = node2.transform.position;

        newPath.transform.position = new Vector3((pos1.x + pos2.x) / 2, (pos1.y + pos2.y) / 2, (pos1.z + pos2.z) / 2);
        return newPath;
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

            // Place a Fight on the first room
            _startingNodes[pathNb].GetComponent<MapNode>().SetRoomTypeTo(RoomType.Combat, resources);
            curRoom = _startingNodes[pathNb].GetComponent<MapNode>()._nextNodes[pathNb].GetComponent<MapNode>();

            while (curRoom.RoomType != RoomType.Boss)
            {
                if (curRoom.RoomType == RoomType.None)
                {
                    curRoom.SetRoomTypeTo(rooms[0], resources);
                    rooms.RemoveAt(0);
                }
                curRoom = curRoom._nextNodes[pathNb].GetComponent<MapNode>();
            }
        }
    }

    bool AvailableDirection(int x, int floor, int nextX, int pathNb)
    {
        if (nextX >= MAP_SIZE_X || nextX < 0)
        {
            return false;
        }

        GameObject[] temp = _mapGrid[nextX][floor].GetComponent<MapNode>()._nextNodes;
        foreach (GameObject item in temp)
        {
            if (ReferenceEquals(item, _mapGrid[x][floor + 1])) return false;
        }

        return true;
    }

    /*GameObject GenerateBlocker(GameObject newPath)
    {
        //print("Added a blocker");
        GameObject blocker = Instantiate(BLOCKER);
        blocker.transform.SetParent(GameObject.FindGameObjectWithTag("Map Door Parent").transform, false);
        Vector3 pos = newPath.transform.position;

        Vector3 point1 = newPath.GetComponent<MapPathScript>()._spline[0].Position;
        Vector3 point2 = newPath.GetComponent<MapPathScript>()._spline[1].Position;
        Vector3 midpoint = (point1 + point2) / 2.0f;

        float3 posE;
        float3 tanE;
        float3 upE;
        if (newPath.GetComponent<MapPathScript>()._spline.Evaluate(0.5f, out posE, out tanE, out upE))
        {
            blocker.transform.position = pos + midpoint;
            blocker.transform.rotation = Quaternion.LookRotation(new Vector3(tanE.x, tanE.y, tanE.z), new Vector3(upE.x, upE.y, upE.z));
        }

        return blocker;
    }*/
}