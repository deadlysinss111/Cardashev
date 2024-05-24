using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Color _startcolor;
    void OnMouseEnter()
    {
        _startcolor = GetComponent<MeshRenderer>().material.color;
        GetComponent<MeshRenderer>().material.color = Color.yellow;
    }
    void OnMouseExit()
    {
        GetComponent<MeshRenderer>().material.color = _startcolor;
    }
}
