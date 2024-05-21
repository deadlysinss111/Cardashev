using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

/* 
 • Tileset class, that will be linked to an in-scene object. The position of said object will be used for the origin of the room's Tileset
 ! This class assumes that, in the scene it is in, there exists one and only one Grid object, with Cell Swizzle: XZY
*/
public class Tileset : MonoBehaviour
{
    /* 
     FIELDS
    */
    // Reference to the Tilemap object this Tileset is linked to
    [SerializeField]
    private Tilemap _tilemap;

    // Clarifies the Zone's type to pull the correct asset and match the aesthetic
    [SerializeField]
    ZoneType _zoneType;


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
    public void OnEnterRoom()
    {
        // TODO: Generate a new Tilemap (for the terrain)
        // TODO: Places Interactible Objects
        // TODO: Places Entities (ennemies and the like)
        // TODO: Places decorations
    }
    // Should be triggered when the room is exited (after a death or an exit)
    public void OnExitRoom()
    {
        // TODO: Fades-in the screen to the Zone map
        // TODO: Purge the entire Tileset and makes it empty
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
