using System;
using System.Collections.Generic;
using UnityEngine;

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

    Animator _animator;

    public GameObject[] _nextNodes;
    public MapBlocker _blocker;
    public GameObject _RoomIcon3D;

    [NonSerialized] public bool _isStartingNode;
    [NonSerialized] public int _startingXCoord;
    //string _linkedScene = "large empty area";
    bool _playerCameThrough;
    bool _isLocked;

    Color _defaultColor;
    Color _defaultHoloColor;
    Color _defaultFresnelColor;
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
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _uniqueNextNode = 0;
        _roomType = RoomType.None;
        _mapNode = GetComponent<GameObject>();
        _isStartingNode = false;
        _playerCameThrough = false;
        _isLocked = false;
    }

    public void AddNextNode(int index, GameObject node)
    {
        int emptyCheck = 0;
        foreach (GameObject item in _nextNodes)
            if (item == null) emptyCheck++;
        if (emptyCheck == _nextNodes.Length) _uniqueNextNode++;

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
        gameObject.SetActive(false);
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
    public bool IsLockedByBlocker()
    {
        return _blocker && _blocker.IsLocked;
    }

    public void SelectNode()
    {
        GetComponent<MeshRenderer>().material.color = Color.cyan;
        _RoomIcon3D.GetComponent<MeshRenderer>().enabled = false; // play a glitch animation + fade transition
        _playerCameThrough = true;

        foreach (GameObject node in _nextNodes)
        {
            if (node == null) continue;
            if (node.GetComponent<MapNode>().IsLockedByBlocker()) continue;
            node.GetComponent<MapNode>().IsSelectable(true);
        }
        /*if(gameObject.name != "Original Node")
        {
            GI._prefabToLoad = _linkedScene;
            SceneManager.LoadScene("TestLvl");
        }*/
    }

    public void UnselectNode()
    {
        GetComponent<MeshRenderer>().material.color = _defaultColor;
        if ( !_playerCameThrough ) _RoomIcon3D.GetComponent<MeshRenderer>().enabled = true;
        foreach (GameObject node in _nextNodes)
        {
            if (node == null) continue;
            node.GetComponent<MapNode>().IsSelectable(false);
        }
    }

    public void SetRoomTypeTo(RoomType roomType, MapResourceLoader resources)
    { 
        _roomType = roomType;
        _RoomIcon3D.GetComponent<MeshRenderer>().gameObject.transform.rotation = Quaternion.identity;
        _defaultHoloColor = new Color(0, 0.52f, 1.498f);
        _defaultFresnelColor = new Color(0f, 0.411f, 2.996f);
        switch (roomType)
        {
            case RoomType.Shop:
                {
                    _RoomIcon3D.GetComponent<MeshFilter>().mesh = resources.SHOP_ICON;
                    Transform temp = _RoomIcon3D.GetComponent<MeshRenderer>().gameObject.transform;
                    temp.localScale *= 2;
                    SetDefaultColorTo(Color.yellow);
                    _defaultHoloColor = new Color(0, 0.52f, 1.498f);
                    _defaultFresnelColor = new Color(0f, 0.411f, 2.996f);
                    break;
                }
            case RoomType.Boss:
                {
                    _RoomIcon3D.GetComponent<MeshFilter>().mesh = resources.BOSS_ICON;
                    _RoomIcon3D.GetComponent<MeshRenderer>().materials[0].SetFloat("_Hologram_Density", 16);
                    Transform temp = _RoomIcon3D.GetComponent<MeshRenderer>().gameObject.transform;
                    temp.localScale *= 2;
                    temp.localPosition = new Vector3(0, 1f, 0);
                    SetDefaultColorTo(Color.black);
                    break;
                }
            case RoomType.Rest:
                {
                    _RoomIcon3D.GetComponent<MeshFilter>().mesh = resources.REST_ICON;
                    SetDefaultColorTo(Color.white);
                    break;
                }
            case RoomType.Event:
                {
                    _RoomIcon3D.GetComponent<MeshFilter>().mesh = resources.EVENT_ICON;
                    _RoomIcon3D.GetComponent<MeshRenderer>().gameObject.transform.rotation = new Quaternion(0, 180, 0, 0);
                    _RoomIcon3D.transform.localScale *= 0.3f;
                    SetDefaultColorTo(Color.green);
                    break;
                }
            case RoomType.Combat:
                {
                    _RoomIcon3D.GetComponent<MeshFilter>().mesh = resources.COMBAT_ICON;
                    SetDefaultColorTo(Color.red);
                    break;
                }
            case RoomType.Elite:
                {
                    _RoomIcon3D.GetComponent<MeshFilter>().mesh = resources.ELITE_ICON;
                    _RoomIcon3D.GetComponent<MeshRenderer>().gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
                    _RoomIcon3D.transform.localScale *= 0.3f;
                    _RoomIcon3D.transform.position += Vector3.up * 2;
                    SetDefaultColorTo(Color.magenta);
                    break;
                }

        }

        _RoomIcon3D.GetComponent<MeshRenderer>().material.SetColor("_Primary_Color", _defaultHoloColor);
        _RoomIcon3D.GetComponent<MeshRenderer>().material.SetColor("_Fresnel_Color", _defaultFresnelColor);
    }

    public void IsSelectable(bool value)
    {
        _animator.SetBool("CanBeSelected", value);
    }

    void SetDefaultColorTo(Color defaultColor)
    {
        GetComponent<MeshRenderer>().material.color = defaultColor;
        _defaultColor = defaultColor;
    }

    public void LockNode()
    {
        if (_playerCameThrough) return;
        _isLocked = true;
        GetComponent<MeshRenderer>().material.color = Color.grey;
        _RoomIcon3D.GetComponent<MeshRenderer>().material.SetColor("_Primary_Color", new Color(0.114f, 0.114f, 0.114f));
        _RoomIcon3D.GetComponent<MeshRenderer>().material.SetColor("_Fresnel_Color", new Color(0.114f, 0.114f, 0.114f));
    }

    public void UnlockNode()
    {
        _isLocked = false;
        GetComponent<MeshRenderer>().material.color = _defaultColor;
        _RoomIcon3D.GetComponent<MeshRenderer>().material.SetColor("_Primary_Color", _defaultHoloColor);
        _RoomIcon3D.GetComponent<MeshRenderer>().material.SetColor("_Fresnel_Color", _defaultFresnelColor);
    }

    private void OnMouseEnter()
    {
        if (_isLocked)
            return;
        if (!_animator.GetBool("MouseHover"))
            _animator.SetBool("MouseHover", true);
    }

    private void OnMouseExit()
    {
        if (_isLocked)
            return;
        _animator.SetBool("MouseHover", false);
    }
}
