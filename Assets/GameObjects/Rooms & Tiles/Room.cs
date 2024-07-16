using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

// TODO: Optimize the code by using Resources.Load() only once per kind-of tile rather than for every instances of tile

public class Room : MonoBehaviour
{
    /* 
     FIELDS
    */
    // Clarifies the Zone's type to pull the correct asset and match the aesthetic
    private ZoneType _zoneType = ZoneType.Radioactive;

    // Essential struct
    private static RoomPrefabEncyclopedia ROOM_ENCYCLOPEDIA = new RoomPrefabEncyclopedia
    (
        new Dictionary<ZoneType, string> {
            { ZoneType.Debug, "Debug" },
            { ZoneType.Radioactive, "Radioactive" },
        },

        new Dictionary<string, RoomPrefabDesc> { }
    );


    /*
     PROPERTIES
    */
    public ZoneType _ZoneType { get; set; }


    /*
     METHODS
    */
    public void EnterRoom(/* prefab name comes from GI*/)
    {
        // Go find the room prefab for the floor
        string path = ROOM_ENCYCLOPEDIA.ZoneFolderName[_zoneType] + " Zone/RoomPrefabs/" + GI._roomType;
        int size = Directory.GetFiles("Assets\\GameObjects\\Rooms & Tiles\\Resources\\" + path).Length;
        GameObject roomPrefab = (GameObject)Resources.Load( path + "/" + UnityEngine.Random.Range(1, size).ToString());

        // Prepares some GameObjects we'll need to instantiate quite a few times during this method
        GameObject gridTemplate = new GameObject();
        gridTemplate.AddComponent<Grid>();
        gridTemplate.GetComponent<Grid>().cellSwizzle = GridLayout.CellSwizzle.XZY;
        GameObject tilemapTemplate = new GameObject();
        tilemapTemplate.AddComponent<Tilemap>();
        tilemapTemplate.AddComponent<TilemapRenderer>();

        List<Transform> populationLayers = new();

        // This allows to instantiate each 3D Model according to the Zone's theme
        // In order, this creates : A new Grid, its Tilemaps, and their tiles for each altitude levels
        int heightLevel = 0;
        foreach (Transform heightGrid in roomPrefab.transform)
        {
            var newGrid = Instantiate(gridTemplate, heightGrid.position, heightGrid.rotation, this.transform);
            newGrid.name = "Height " + heightLevel;

            foreach (Transform tilemap in heightGrid.transform)
            {
                GameObject newTilemap = Instantiate(tilemapTemplate, tilemap.position, tilemap.rotation, newGrid.transform);
                newTilemap.name = tilemap.name;

                if (tilemap.gameObject.name == "Population")
                {
                    populationLayers.Add(tilemap);
                    continue;
                }

                LoadHeight(tilemap, newTilemap);
            }
            heightLevel++;
        }

        heightLevel = 0;

        NavMeshSurface surface = GameObject.Find("RoomAnchor").AddComponent<NavMeshSurface>();
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        surface.BuildNavMesh();

        Transform parent = GameObject.Find("Height 1").transform;
        foreach (Transform heightGrid in populationLayers)
        {
            /*var newGrid = Instantiate(gridTemplate, heightGrid.position, heightGrid.rotation, this.transform);
            newGrid.name = "Height " + heightLevel;*/

            Transform tilemap = heightGrid.transform;
            GameObject newTilemap = Instantiate(tilemapTemplate, tilemap.position, tilemap.rotation, parent);
            newTilemap.name = tilemap.name;
            LoadHeight(tilemap, newTilemap);
            heightLevel++;
        }

        // Cleanup. Avoids having them instantiated one too many times
        Destroy(gridTemplate);
        Destroy(tilemapTemplate);

        // TODO: Places Entities (ennemies and the like)
    }

    void LoadHeight(Transform tilemap, GameObject newTilemap)
    {
        foreach (Transform roomGO in tilemap.transform)
        {
            // Finds the correct path to the asset, taking into accounts the Zone Type and asset type
            string roomGOpath = "";
            switch (tilemap.name)
            {
                case "Interactibles":
                    roomGOpath = ROOM_ENCYCLOPEDIA.ZoneFolderName[_zoneType] + " Zone/" + tilemap.name + "/Prefabs/" + roomGO.name;
                    break;

                case "Population":
                    roomGOpath = roomGO.name + "/" + roomGO.name;
                    //Debug.Log("Attempt to load the prefab " + roomGOpath);
                    break;

                // Topology and Decoration tilemaps
                default:
                    roomGOpath = ROOM_ENCYCLOPEDIA.ZoneFolderName[_zoneType] + " Zone/" + tilemap.name + "/" + roomGO.name;
                    break;
            }
            //Debug.Log("Attempt to load the prefab " + roomGOpath);

            //print(roomGOpath);
            // We use the ROOMGO to get the local position of the object, and the roomGO for the global transform
            GameObject ROOMGO = (GameObject)Resources.Load(roomGOpath);

            // I cannot understand the reason of it but if we dont offset the ROOMGO.transform by a vector that goes down, the preview become bugged...
            Vector3 buf = new Vector3(0, 0, 0) + ROOMGO.transform.position;
            Instantiate(ROOMGO, roomGO.transform.position + buf, roomGO.transform.rotation, newTilemap.transform);
        }
    }

    void Awake()
    {
        // Loads the room the player entered and bakes its surface
        EnterRoom();
        NavMeshSurface surface = GameObject.Find("RoomAnchor").AddComponent<NavMeshSurface>();
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        surface.BuildNavMesh();
    }
}
