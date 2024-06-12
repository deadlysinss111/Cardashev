using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomClear : MonoBehaviour
{
    public bool _roomClearScreen = false;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        //if (gameObject.activeInHierarchy == false)
        _roomClearScreen = true;
    }
}
