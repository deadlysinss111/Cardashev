using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnterTestLvl : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Room room = new Room();
        room.OnEnterRoom(GlobalInformations._prefabToLoadOnRoomEnter);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
