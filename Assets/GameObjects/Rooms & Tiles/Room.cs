using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

// TODO: Optimize the code by using Resources.Load() only once per kind-of tile rather than for every instances of tile

/* 
 • Room class, that will be linked to an in-scene object. The position of said object will be used for the origin of the room
*/
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
        new Dictionary<ZoneType, string>
        {
            { ZoneType.Debug, "Debug" }
        },
        new Dictionary<string, RoomPrefabDesc>
        { }
    );


    /*
     PROPERTIES
    */
    public ZoneType _ZoneType { get; set; }


    /*
     EVENTS
    */
    // Nothing yet


    /*
     METHODS
    */
    public void EnterRoom(/* prefab name comes from GI*/)
    {
        // Go find and instantiate the room prefab for the floor
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

                foreach (Transform model in tilemap.transform)
                {
                    // Finds the correct path to the asset, taking into accounts the Zone Type and asset type
                    string modelPath = "";
                    if(tilemap.name == "Interactibles")
                        modelPath = ROOM_ENCYCLOPEDIA.ZoneFolderName[_zoneType] + " Zone/" + tilemap.name + "/Prefabs/" + model.name;
                    else
                        modelPath = ROOM_ENCYCLOPEDIA.ZoneFolderName[_zoneType] + " Zone/" + tilemap.name + "/" + model.name;
                    //Debug.Log("Attempt to load the prefab " + modelPath);

                    GameObject MODEL = (GameObject)Resources.Load(modelPath);

                    // I cannot understand the reason of it but if we dont offset the MODEL.trnsform by a vector that goes down, the preview become bugged...
                    Vector3 buf = new Vector3(0, -1, 0) + MODEL.transform.position;
                    Instantiate(MODEL, model.transform.position + buf , model.transform.rotation, newTilemap.transform);
                }
            }
            heightLevel++;
        }

        // Cleanup. Avoids having them instantiated one too many times
        Destroy(gridTemplate);
        Destroy(tilemapTemplate);

        // TODO: Places Entities (ennemies and the like)
    }

    public void ExitRoom()
    {
        // Nothing until retrospective
    }

    // Generic method to get a child by name
    public GameObject FindChild(string ARGchildName)
    {
        foreach (Transform child in this.transform)
            if (child.name == ARGchildName)
                return child.gameObject;
        return null;
    }

    // Genric methods to get a child recusively by name. Gift of ChatGPT
    public GameObject FindChildRecursively(string ARGchildName)
    {
        return INTERNALFindChildRec(this.transform, ARGchildName);
    }

    private GameObject INTERNALFindChildRec(Transform parent, string ARGchildName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == ARGchildName)
                return child.gameObject;

            GameObject result = INTERNALFindChildRec(child, ARGchildName);
            if (result != null)
                return result;
        }
        return null;
    }


    void Awake()
    {
        /* Old way of creating the Encyclopedias
        Dictionary<ZoneType, string> ZoneFolderName = new Dictionary<ZoneType, string>
        {
            { ZoneType.Debug, "Debug"}
        };

        Dictionary<string, RoomPrefabDesc> RoomBook = new Dictionary<string, RoomPrefabDesc>();

        ROOM_ENCYCLOPEDIA = new RoomPrefabEncyclopedia(ZoneFolderName, RoomBook);
        */

        // Loads the room the player entered and bakes its surface
        EnterRoom();
        NavMeshSurface surface = GameObject.Find("RoomAnchor").AddComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }


    // CODE TO BE MOVED IN THE REWARD CLASS
}
