using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

/* 
 • Room class, that will be linked to an in-scene object. The position of said object will be used for the origin of the room
 ! This class assumes that, in the scene it is in, there exists one and only one Grid object, with Cell Swizzle: XZY
*/
public class Room : MonoBehaviour
{
    /* 
     FIELDS
    */
    // Reference to the Tilemaps objects this Room is linked to
    [SerializeField]
    private Tilemap _TMtopology;
    private Tilemap _TMinteractibles;
    private Tilemap _TMdecorations;

    // Clarifies the Zone's type to pull the correct asset and match the aesthetic
    [SerializeField]
    private ZoneType _zoneType = ZoneType.Debug;

    // Essential structs
    private RoomPrefabEncyclopedia _roomEncyclopedia;


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
        Object roomPrefab = Resources.Load(_roomEncyclopedia.ZoneFolderName[_zoneType] + " Zone/" + ARGprefabName);

        GameObject newFloor = (GameObject)Instantiate(roomPrefab);
        _TMtopology = newFloor.GetComponent<Tilemap>();

        // TODO: Places Interactible Objects
        // TODO: Places Entities (ennemies and the like)
        // TODO: Places decorations
    }
    // Should be triggered when the room is exited (after a death or an exit)
    public void OnExitRoom()
    {
        // TODO: Fades-in the screen to the Zone map

        // Empties all Tilemaps from the Room Anchor
        foreach(Transform tile in _TMtopology.transform)
        {
            Destroy(tile.gameObject);
        }
    }


    void Start()
    {
        // • Creates the RoomPrefabEncyclopedia for the Room class. This is done by filling 2 Dictionaries, and instantiating the struct.
        // ! TODO: Fill the second Dictionary with actual data when we'll want to compose room with multiple prefabs
        Dictionary<ZoneType, string> ZoneFolderName = new Dictionary<ZoneType, string>();
        ZoneFolderName.Add(ZoneType.Debug, "Debug");

        Dictionary<string, RoomPrefabDesc> RoomBook = new Dictionary<string, RoomPrefabDesc>();

        this._roomEncyclopedia = new RoomPrefabEncyclopedia(ZoneFolderName, RoomBook);

        this._zoneType = ZoneType.Debug;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E was pressed, trying to generate L Room...");
            OnEnterRoom("L Room");
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("L was pressed, trying to destroy L Room...");
            OnExitRoom();
        }
    }
}
