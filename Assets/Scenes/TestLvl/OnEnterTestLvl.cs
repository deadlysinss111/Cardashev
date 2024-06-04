using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class OnEnterTestLvl : MonoBehaviour
{
    void Start()
    {
        // Deactivate the Zone'a map object
        GameObject.Find("Map(Clone)").SetActive(false);

        // Loads in the room corresponding to the node
        Room room = gameObject.GetComponent<Room>();
        room.OnEnterRoom(GI._prefabToLoadOnRoomEnter);

        // Bakes the walkable surface
        NavMeshSurface surface = GameObject.Find("RoomAnchor").AddComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }
}
