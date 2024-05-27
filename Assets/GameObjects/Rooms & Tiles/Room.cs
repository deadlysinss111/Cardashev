using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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

    // Essential structs
    private static RoomPrefabEncyclopedia ROOM_ENCYCLOPEDIA;


    /*
     PROPERTIES
    */
    public ZoneType _ZoneType { get; set; }


    /*
     EVENTS
    */
    // Nothing for now


    /*
     METHODS
    */

    // Should be triggered when the room is entered for the first time
    public void OnEnterRoom(string ARGprefabName)
    {
        // Go find and instantiate the room prefab for the floor
        GameObject roomPrefab = (GameObject)Resources.Load(ROOM_ENCYCLOPEDIA.ZoneFolderName[_zoneType] + " Zone/" + ARGprefabName);

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
                        modelPath = ROOM_ENCYCLOPEDIA.ZoneFolderName[_zoneType] + " Zone/" + tilemap.name + "/3D Models/" + model.name;
                    else
                        modelPath = ROOM_ENCYCLOPEDIA.ZoneFolderName[_zoneType] + " Zone/" + tilemap.name + "/" + model.name;
                    Debug.Log("Attempt to load the prefab " + modelPath);
                    Instantiate(Resources.Load(modelPath), model.transform.position, model.transform.rotation, newTilemap.transform);
                }
            }
            heightLevel++;
        }

        // Cleanup. Avoids having them instantiated one too many times
        Destroy(gridTemplate);
        Destroy(tilemapTemplate);

        // TODO: Places Entities (ennemies and the like)
    }
    // Should be triggered when the room is exited (after a death or an exit)
    public void OnExitRoom()
    {
        // Empties all Tilemaps from the Room Anchor
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
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


    void Start()
    {
        // • Creates the RoomPrefabEncyclopedia for the Room class. This is done by filling 2 Dictionaries, and instantiating the struct.
        // ! TODO: Fill the second Dictionary with actual data when we'll want to compose room with multiple prefabs
        Dictionary<ZoneType, string> ZoneFolderName = new Dictionary<ZoneType, string>();
        ZoneFolderName.Add(ZoneType.Debug, "Debug");

        Dictionary<string, RoomPrefabDesc> RoomBook = new Dictionary<string, RoomPrefabDesc>();

        ROOM_ENCYCLOPEDIA = new RoomPrefabEncyclopedia(ZoneFolderName, RoomBook);

        _zoneType = ZoneType.Debug;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Room Loading attempt...");
            OnEnterRoom("TestRoom");
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Room Unloading attempt...");
            OnExitRoom();
        }
    }
}
