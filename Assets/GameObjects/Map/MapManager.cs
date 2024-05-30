using System.Collections;
using System.Collections.Generic;
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

    // PREFABS
    GameObject MAP_NODE;
    GameObject MAP_PATH;
    
    // For map navigation
    List<List<GameObject>> _mapGrid;
    GameObject _playerLocation; // TODO auto create this one and make it invisible at Start()

    // Miscenalious
    [SerializeField] LayerMask _clickableLayers;

    void Start()
    {
        _mapGrid = GlobalInformations._mapNodes;

        if (_mapGrid != null)
        {
            return;
        }

        _mapSizeX = 7;
        _mapSizeY = 15;

        _mapGrid = new List<List<GameObject>>(_mapSizeX);
        _startingNodes = new List<GameObject>();

        MAP_NODE = (GameObject)Resources.Load("Map Node");
        MAP_PATH = (GameObject)Resources.Load("Map Path");
        // Generate an invisible starting node
        _playerLocation = Instantiate(MAP_NODE).transform.gameObject;
        _playerLocation.GetComponent<MapNode>().SetAsOriginalNode();

        GenerateMap();
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
                obj.transform.SetParent(mapObj, false);
                obj.transform.localPosition = new Vector3(i * spaceBetweenNodes, 4, j * spaceBetweenNodes);
            }
        }
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
        for (int y = 0; y < 4; y++)
        {
            int newStartCoordIndex = Random.Range(0, freeNodelist.Count);
            GameObject startingNode = freeNodelist[newStartCoordIndex];
            startingNode.GetComponent<MapNode>()._isStartingNode = true;
            startingNode.name = "Starting Node";
            _playerLocation.GetComponent<MapNode>()._nextNodes.Add(startingNode);
            _startingNodes.Add(startingNode);
            freeNodelist.RemoveAt(newStartCoordIndex);
        }

        foreach (GameObject startingNode in _startingNodes)
        {
            int x = startingNode.GetComponent<MapNode>()._startingXCoord;
            for (int floorNb = 0; floorNb < _mapSizeY-1; floorNb++)
            {
                int nextNodeXIndex = Mathf.Clamp(Random.Range(x - 1, x + 2), 0, _mapSizeX - 1);
                _mapGrid[x][floorNb].GetComponent<MapNode>()._nextNodes.Add(_mapGrid[nextNodeXIndex][floorNb+1]);

                // TODO placing paths between nodes
                /*GameObject newPath = Instantiate(MAP_PATH);
                newPath.transform.SetParent(GameObject.FindGameObjectWithTag("Map Path Parent").transform, false);
                newPath.GetComponentInChildren<MapPathScript>().SetPathPoints(_mapGrid[x][floorNb], _mapGrid[nextNodeXIndex][floorNb + 1]);*/

                // Saving the x coordinate of the next node for the next iteration of the loop
                x = nextNodeXIndex;
            }
        }

        // Disabling path-less nodes to make them invisible
        for (int i = 0; i < _mapSizeX; i++)
        {
            for (int j = 0; j < _mapSizeY; j++)
            {
                if (_mapGrid[i][j].GetComponent<MapNode>()._nextNodes.Count == 0)
                {
                    _mapGrid[i][j].SetActive(false);
                }
                if (_mapGrid[i][j].GetComponent<MapNode>()._isStartingNode)
                {
                    //_mapGrid[i][j].SetActive(true);
                }
            }
        }
    }

    bool ArePathsCrossing(MapNode room1, MapNode room2)
    {

        return false;
    }
}
