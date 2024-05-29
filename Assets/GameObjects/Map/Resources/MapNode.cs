using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MapNode : MonoBehaviour
{
    [NonSerialized] public GameObject _mapNode;
    public List<GameObject> _nextNodes;

    [NonSerialized] public bool _isStartingNode;
    [NonSerialized] public int _startingXCoord;

    private void Start()
    {
        _mapNode = GetComponent<GameObject>();
        GetComponent<MeshRenderer>().material.color = Color.blue;
        _isStartingNode = false;
    }

    public void SetAsOriginalNode()
    {
        transform.SetParent(GameObject.FindGameObjectsWithTag("Map")[0].transform, false);
        transform.localPosition = new Vector3(1, 3);
        transform.name = "Original Node";
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void SelectNode()
    {
        print("Select Node");
        GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    public void UnselectNode()
    {
        if (_isStartingNode)
        {
            return;
        }
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    public void LockNode()
    {
        if (_isStartingNode)
        {
            return;
        }
        GetComponent<MeshRenderer>().material.color = Color.grey;
    }
}
