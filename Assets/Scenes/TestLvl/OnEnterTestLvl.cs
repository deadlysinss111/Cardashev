using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class OnEnterTestLvl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Map(Clone)").SetActive(false);
        Room room = gameObject.GetComponent<Room>();
        room.OnEnterRoom(GlobalInformations._prefabToLoadOnRoomEnter);
        NavMeshSurface surface = GameObject.Find("RoomAnchor").AddComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }
}
