using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    Color _startcolor;
    PlayerManager _playerManager;

    bool _selectable = false;
    public bool IsSelectable { get { return _selectable; } }

    void OnMouseEnter()
    {
        //_startcolor = GetComponent<MeshRenderer>().material.color;

        if (gameObject.layer == 9 && false == GI._PManFetcher()._isWallClickable)
            return;

        //GetComponent<MeshRenderer>().material.color = Color.yellow;
        GI._PManFetcher().TriggerMouseHovering();
    }
    void OnMouseExit()
    {
        //GetComponent<MeshRenderer>().material.color = _startcolor;
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
            outline.OutlineMode = Outline.Mode.OutlineFuckAround;
            outline.OutlineColor = Color.red;
            outline.OutlineWidth = 7.7f;
        }
        else if (_selectable == false && TryGetComponent<Outline>(out outline))
        {
            Destroy(outline);
        }
    }
}