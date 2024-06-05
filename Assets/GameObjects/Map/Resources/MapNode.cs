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
    public MapBlocker _blocker;

    [NonSerialized] public bool _isStartingNode;
    [NonSerialized] public int _startingXCoord;
    string _linkedScene = "large empty area";
    bool _playerCameThrough;

    Color _defaultColor;
    public RoomType _roomType;
    public RoomType RoomType
    {
        get => _roomType;
    }
    private int _uniqueNextNode;
    public int UniqueNextNode
    {
        get => _uniqueNextNode;
    }

    private void Awake()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;        
    }

    private void Start()
    {
        _uniqueNextNode = 0;
        _roomType = RoomType.None;
        _mapNode = GetComponent<GameObject>();
        _isStartingNode = false;
        _playerCameThrough = false;
    }

    public void AddNextNode(int index, GameObject node)
    {
        int emptyCheck = 0;
        foreach (GameObject item in _nextNodes)
            if (item == null) emptyCheck++;
        if (emptyCheck == 4) _uniqueNextNode++;

        _nextNodes[index] = node;

        foreach (GameObject item in _nextNodes)
        {
            if (item == null)
            {
                continue;
            }
            if (!ReferenceEquals(item, node))
            {
                _uniqueNextNode++;
            }
        }
    }

    public void SetAsOriginalNode()
    {
        transform.SetParent(GameObject.FindGameObjectsWithTag("Map")[0].transform, false);
        transform.localPosition = new Vector3(1, 3);
        transform.name = "Original Node";
        GetComponent<MeshRenderer>().enabled = false;
    }

    public int NumberOfNextNode()
    {
        int pathCount = 0;
        foreach (GameObject node in _nextNodes)
        {
            if (node == null)
                continue;
            else
            {
                List<GameObject> temp = new List<GameObject>();
                foreach (GameObject otherNode in _nextNodes)
                {
                    if (ReferenceEquals(node, otherNode))
                        continue;

                    temp.Add(otherNode);
                }
                if (!temp.Contains(node))
                    pathCount++;
            }
        }
        return pathCount;
    }

    public void SelectNode()
    {
        GetComponent<MeshRenderer>().material.color = Color.cyan;
        _playerCameThrough = true;
        //GlobalInformations._prefabToLoadOnRoomEnter = _linkedScene;
        //SceneManager.LoadScene("TestLvl");
    }

    public void UnselectNode()
    {
        GetComponent<MeshRenderer>().material.color = _defaultColor;
    }

    public void SetRoomTypeTo(RoomType roomType)
    { 
        _roomType = roomType;
        switch (roomType)
        {
            case RoomType.Shop:
                SetDefaultColorTo(Color.yellow);
                break;
            case RoomType.Boss:
                SetDefaultColorTo(Color.black);
                break;
            case RoomType.Rest:
                SetDefaultColorTo(Color.white);
                break;
            case RoomType.Event:
                SetDefaultColorTo(Color.green);
                break;
            case RoomType.Combat:
                SetDefaultColorTo(Color.red);
                break;
            case RoomType.Elite:
                SetDefaultColorTo(Color.magenta);
                break;

        }
    }

    void SetDefaultColorTo(Color defaultColor)
    {
        GetComponent<MeshRenderer>().material.color = defaultColor;
        _defaultColor = defaultColor;
    }

    public void LockNode()
    {
        if (_playerCameThrough) return;
        GetComponent<MeshRenderer>().material.color = Color.grey;
    }
}
