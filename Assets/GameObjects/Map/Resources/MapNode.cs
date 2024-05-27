using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MapNode : MonoBehaviour
{
    [NonSerialized] public GameObject _mapNode;
    public List<GameObject> _nextNodes;
    GameObject _nodePath1;
    GameObject _nodePath2;

    public bool _isStartRoom;

    private void Start()
    {
        _mapNode = GetComponent<GameObject>();
        GetComponent<MeshRenderer>().material.color = Color.blue;
        _isStartRoom = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetAsStartingNode()
    {
        _isStartRoom = true;
        GetComponent<MeshRenderer>().material.color = Color.clear;
    }

    public void SelectNode()
    {
        print("Node selected !");
        if (_isStartRoom)
        {
            return;
        }
        GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    public void UnselectNode()
    {
        if (_isStartRoom)
        {
            return;
        }
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void LockNode()
    {
        if (_isStartRoom)
        {
            return;
        }
        GetComponent<MeshRenderer>().material.color = Color.grey;
    }
}
