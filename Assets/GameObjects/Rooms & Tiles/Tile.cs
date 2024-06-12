using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    Color _startcolor;  // DEPRECATED !!! Must be highlighted differently in the future, since we shouldn't touch the material

    void OnMouseEnter()
    {
        _startcolor = GetComponent<MeshRenderer>().material.color;
        GetComponent<MeshRenderer>().material.color = Color.yellow;
        GI._PManFetcher().TriggerMouseHovering();
    }
    void OnMouseExit()
    {
        GetComponent<MeshRenderer>().material.color = _startcolor;
    }
}