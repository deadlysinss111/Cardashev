using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    Color _startcolor;
    PlayerManager _playerManager;

    public bool _selectable = false;

    private void Awake()
    {
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
    }
    void OnMouseEnter()
    {
        _startcolor = GetComponent<MeshRenderer>().material.color;
        GetComponent<MeshRenderer>().material.color = Color.yellow;
        _playerManager.TriggerMouseHovering();
    }
    void OnMouseExit()
    {
        GetComponent<MeshRenderer>().material.color = _startcolor;
    }
    private void Update()
    {
        Outline outline;
        if (_selectable && TryGetComponent<Outline>(out outline) == false)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.cyan;
            outline.OutlineWidth = 4.7f;
        }
        else if (_selectable == false && TryGetComponent<Outline>(out outline))
        {
            Destroy(outline);
        }
    }
}