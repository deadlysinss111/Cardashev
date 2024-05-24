using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    [NonSerialized] public GameObject _mapNode;
    public List<GameObject> _nextNodes;

    public bool _isStartRoom;

    private void Start()
    {
        _mapNode = GetComponent<GameObject>();
        _isStartRoom = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectNode()
    {
        print("Node selected !");
        GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    public void UnselectNode()
    {
        GetComponent<MeshRenderer>().material.color = Color.white;
    }
}
