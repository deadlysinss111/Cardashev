using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    Color _startcolor;
    PlayerManager _playerManager;

    bool _selectable = false;
    public bool IsSelectable { get { return _selectable; } }
    public bool _isSingle = false;

    void OnMouseEnter()
    {
        //_startcolor = GetComponent<MeshRenderer>().material.color;

        if (gameObject.layer == 9 && false == GI._PManFetcher()._isWallClickable)
            return;

        //GetComponent<MeshRenderer>().material.color = Color.yellow;
        GI._PManFetcher().TriggerMouseHovering();

        if (_selectable && TryGetComponent(out Outline outline) == true)
        {
            outline.isSingle = true;
            _isSingle = true;
            outline.OutlineColor = Color.blue;
            outline.needsUpdate = true;
        }
        else
        {
            _isSingle = true;
            outline = gameObject.AddComponent<Outline>();
            outline.isSingle = true;
            outline.OutlineMode = Outline.Mode.Test;
            outline.OutlineColor = Color.blue;
            outline.OutlineWidth = 7.7f;
        }
    }
    void OnMouseExit()
    {
        //GetComponent<MeshRenderer>().material.color = _startcolor;
        Outline outline = GetComponent<Outline>();
        if (_selectable)
        {
            outline.OutlineColor = Color.red;
            outline.needsUpdate = true;
        }
        else
        {
            Destroy(outline);
        }
    }

    public void SetSelected(bool value, bool hitWall = false)
    {
        if (gameObject.layer == 9 && false == hitWall)
            return;

        _selectable = value;

        Outline outline;
        if (_selectable && TryGetComponent<Outline>(out _) == false)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.Test;
            outline.OutlineColor = Color.red;
            outline.OutlineWidth = 7.7f;
        }
        else if (_selectable == false && TryGetComponent<Outline>(out outline))
        {
            Destroy(outline);
        }
    }
}