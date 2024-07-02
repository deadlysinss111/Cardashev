using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

// TODO: Optimize the code by using Resources.Load() only once per kind-of tile rather than for every instances of tile

public class Room : MonoBehaviour
{
    /* 
     FIELDS
    */
    // Clarifies the Zone's type to pull the correct asset and match the aesthetic
    private ZoneType _zoneType = ZoneType.Debug;

    // Essential struct
    private static RoomPrefabEncyclopedia ROOM_ENCYCLOPEDIA = new RoomPrefabEncyclopedia
    (
        new Dictionary<ZoneType, string> {
            { ZoneType.Debug, "Debug" }},

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
        GameObject roomPrefab = (GameObject)Resources.Load(ROOM_ENCYCLOPEDIA.ZoneFolderName[_zoneType] + " Zone/" + GI._prefabToLoad);

        // Prepares some GameObjects we'll need to instantiate quite a few times during this method
        GameObject gridTemplate = new GameObject();
        gridTemplate.AddComponent<Grid>();
        gridTemplate.GetComponent<Grid>().cellSwizzle = GridLayout.CellSwizzle.XZY;
        GameObject tilemapTemplate = new GameObject();
        tilemapTemplate.AddComponent<Tilemap>();
        tilemapTemplate.AddComponent<TilemapRenderer>();

        // This allows to instantiate each 3D Model according to the Zone's theme
        // In order, this creates : A new Grid, its Tilemaps, and their tiles for each altitude levels
        int heightLevel = 0;
        foreach (Transform heightGrid in roomPrefab.transform)
        {
            var newGrid = Instantiate(gridTemplate, heightGrid.position, heightGrid.rotation, this.transform);
            newGrid.name = "Height " + heightLevel;

            foreach (Transform tilemap in heightGrid.transform)
            {
                var newTilemap = Instantiate(tilemapTemplate, tilemap.position, tilemap.rotation, newGrid.transform);
                newTilemap.name = tilemap.name;

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
                            break;

                        // Topology and Decoration tilemaps
                        default:
                            roomGOpath = ROOM_ENCYCLOPEDIA.ZoneFolderName[_zoneType] + " Zone/" + tilemap.name + "/" + roomGO.name;
                            break;
                    }
                    //Debug.Log("Attempt to load the prefab " + modelPath);

                    // We use the MODEL to get the local position of the object, and the model for the global transform
                    GameObject ROOMGO = (GameObject)Resources.Load(roomGOpath);

                    // I cannot understand the reason of it but if we dont offset the MODEL.transform by a vector that goes down, the preview become bugged...
                    Vector3 buf = new Vector3(0, 0, 0) + ROOMGO.transform.position;
                    Instantiate(ROOMGO, roomGO.transform.position + buf , roomGO.transform.rotation, newTilemap.transform);
                }
            }
            heightLevel++;
        }

        // Cleanup. Avoids having them instantiated one too many times
        Destroy(gridTemplate);
        Destroy(tilemapTemplate);

        // TODO: Places Entities (ennemies and the like)
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
