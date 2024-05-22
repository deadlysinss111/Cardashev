using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    int _mapSizeX;
    int _mapSizeY;

    class MapNode
    {
        public List<Vector2> _prevRooms;

        public MapNode()
        {

        }

        public MapNode(List<Vector2> nextRooms)
        {
            _prevRooms = nextRooms;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _mapSizeX = 7;
        _mapSizeY = 15;
    }

    void GenerateMap()
    {
        List<List<MapNode>> mapGrid = new List<List<MapNode>>();

        for (int i = 0; i < _mapSizeY; i++)
        {
            for (int j = 0; j < _mapSizeX; j++)
            {
                mapGrid[i][j] = new MapNode();
            }
        }

        // WIP Path generation
        https://steamcommunity.com/sharedfiles/filedetails/?id=2830078257
        Vector2 prevRoom = new Vector2();
        for (int pathNumber = 0; pathNumber < 6; pathNumber++)
        {
            for (int floor = 1; floor < _mapSizeY; floor++)
            {
                
            }
            Random.Range(0, 3);
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
        }
    }
}
