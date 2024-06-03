using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public enum RoomType
{
    None,
    Shop,
    Boss,
    Rest,
    Event,
    Combat,
    Elite
}

public class MapNode : MonoBehaviour
{
    [NonSerialized] public GameObject _mapNode;

    public GameObject[] _nextNodes;

    [NonSerialized] public bool _isStartingNode;
    [NonSerialized] public int _startingXCoord;
    string _linkedScene = "large empty area";

    private RoomType _roomType;
    public RoomType RoomType
    { 
        get => _roomType;
    }

    private void Awake()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;        
    }

    private void Start()
    {
        _roomType = RoomType.None;
        _mapNode = GetComponent<GameObject>();
        _isStartingNode = false;
    }

    public void SetAsOriginalNode()
    {
        transform.SetParent(GameObject.FindGameObjectsWithTag("Map")[0].transform, false);
        transform.localPosition = new Vector3(1, 3);
        transform.name = "Original Node";
        GetComponent<MeshRenderer>().enabled = false;
    }

    public bool HasNextNode()
    {
        foreach (var node in _nextNodes)
        {
            if (node == null)
                continue;
            else
                return true;
        }
        return false;
    }

    public void SelectNode()
    {
        GetComponent<MeshRenderer>().material.color = Color.yellow;
        GlobalInformations._prefabToLoadOnRoomEnter = _linkedScene;
        SceneManager.LoadScene("TestLvl");
    }

    public void UnselectNode()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void SetRoomTypeTo(RoomType roomType)
    { 
        _roomType = roomType;
        switch (roomType)
        {
            case RoomType.Shop:
                GetComponent<MeshRenderer>().material.color = Color.yellow;
                break;
            case RoomType.Boss:
                GetComponent<MeshRenderer>().material.color = Color.black;
                break;
            case RoomType.Rest:
                GetComponent<MeshRenderer>().material.color = Color.white;
                break;
            case RoomType.Event:
                GetComponent<MeshRenderer>().material.color = Color.green;
                break;
            case RoomType.Combat:
                GetComponent<MeshRenderer>().material.color = Color.red;
                break;
            case RoomType.Elite:
                GetComponent<MeshRenderer>().material.color = Color.magenta;
                break;

        }
    }

    public void LockNode()
    {
        GetComponent<MeshRenderer>().material.color = Color.grey;
    }
}
