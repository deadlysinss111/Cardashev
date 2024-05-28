using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    Color _startcolor;
    PlayerManager _playerManager;

    private void Awake()
    {
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
    }
    void OnMouseEnter()
    {
        _startcolor = GetComponent<MeshRenderer>().material.color;
        GetComponent<MeshRenderer>().material.color = Color.yellow;
        _playerManager.ExecuteCurrentStateAction();
    }
    void OnMouseExit()
    {
        GetComponent<MeshRenderer>().material.color = _startcolor;
    }
}
