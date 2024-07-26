using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

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

    public GameObject[] _nextNodes;
    public MapBlocker _blocker;

    // Objects for the room icon
    [SerializeField] GameObject _ScaleParent;
    public GameObject _RoomIcon3D;
    public Sprite _roomIconSprite;

    [NonSerialized] public bool _isStartingNode;
    [NonSerialized] public int _startingXCoord;
    public string _stringType;
    //string _linkedScene = "Shop";
    public bool _playerCameThrough;
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
        //_animator = GetComponent<Animator>();
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
        return _blocker && _blocker._IsLocked;
    }

    public void LoadRoom()
    {
        if (_stringType == "Event") return;
        if (gameObject.name != "Original Node")
        {
            GI._currentRoomIcon = _roomIconSprite;
            GameObject.Find("Transition Effect").GetComponent<SceneLoadingAnimation>().StartAnimation(_stringType);
        }
    }

    public void SelectNode()
    {
        GetComponent<MeshRenderer>().material.color = Color.cyan;
        //_RoomIcon3D.GetComponent<MeshRenderer>().enabled = false; // play a glitch animation + fade transition
        _playerCameThrough = true;
    }

    public void UnselectNode()
    {
        GetComponent<MeshRenderer>().material.color = _defaultColor;
        if ( !_playerCameThrough )
            _RoomIcon3D.GetComponent<MeshRenderer>().enabled = true;
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
                    _roomIconSprite = resources.SHOP_ICON_SPRITE;
                    _RoomIcon3D.GetComponent<MeshFilter>().mesh = resources.SHOP_ICON;
                    _RoomIcon3D.GetComponent<MeshRenderer>().gameObject.transform.rotation = new Quaternion(0, 180, 0, 0);
                    _ScaleParent.transform.localPosition = new Vector3(0, 1.1f, 0);
                    _ScaleParent.transform.localScale = new Vector3(.3f, .3f, .3f);
                    _defaultHoloColor = new Color(1.498f, 1.073f, 0f);
                    _defaultFresnelColor = new Color(2.996f, 2.3f, 0f);
                    SetDefaultColorTo(Color.yellow);
                    _stringType = "Shop";
                    break;
                }
            case RoomType.Boss:
                {
                    _roomIconSprite = resources.BOSS_ICON_SPRITE;
                    _RoomIcon3D.GetComponent<MeshFilter>().mesh = resources.BOSS_ICON;
                    _RoomIcon3D.GetComponent<MeshRenderer>().materials[0].SetFloat("_Hologram_Density", 16);
                    _ScaleParent.transform.localPosition = new Vector3(-0.07f, -0.2f, 0);
                    _ScaleParent.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
                    GetComponent<BoxCollider>().size = new Vector3(2, 4.8f, 0);
                    SetDefaultColorTo(Color.black);
                    _stringType = "Boss";
                    break;
                }
            case RoomType.Rest:
                {
                    _roomIconSprite = resources.REST_ICON_SPRITE;
                    _RoomIcon3D.GetComponent<MeshFilter>().mesh = resources.REST_ICON;
                    _RoomIcon3D.GetComponent<MeshRenderer>().gameObject.transform.rotation = new Quaternion(0, 180, 0, 0);
                    _ScaleParent.transform.localPosition = new Vector3(0, 1.1f, 0);
                    _ScaleParent.transform.localScale = new Vector3(.45f, .45f, .45f);
                    _defaultHoloColor = new Color(0f, 3f, 0f);
                    _defaultFresnelColor = new Color(0.092f, 1.5f, 0.43f);
                    SetDefaultColorTo(Color.white);
                    _stringType = "Combat";
                    break;
                }
            case RoomType.Event:
                {
                    _roomIconSprite = resources.EVENT_ICON_SPRITE;
                    _RoomIcon3D.GetComponent<MeshFilter>().mesh = resources.EVENT_ICON;
                    _RoomIcon3D.GetComponent<MeshRenderer>().gameObject.transform.rotation = new Quaternion(0, 180, 0, 0);
                    _ScaleParent.transform.localPosition = new Vector3(-.1f, .4f, 0);
                    _ScaleParent.transform.localScale = new Vector3(.3f, .3f, .3f);
                    SetDefaultColorTo(Color.green);
                    _stringType = "Event";
                    break;
                }
            case RoomType.Combat:
                {
                    _roomIconSprite = resources.COMBAT_ICON_SPRITE;
                    _RoomIcon3D.GetComponent<MeshFilter>().mesh = resources.COMBAT_ICON;
                    SetDefaultColorTo(Color.red);
                    _stringType = "Combat";
                    break;
                }
            case RoomType.Elite:
                {
                    _roomIconSprite = resources.ELITE_ICON_SPRITE;
                    _RoomIcon3D.GetComponent<MeshFilter>().mesh = resources.ELITE_ICON;
                    _ScaleParent.transform.localPosition = new Vector3(-.04f, 1.25f, 0);
                    _ScaleParent.transform.localScale = new Vector3(.3f, .3f, .3f);
                    SetDefaultColorTo(Color.magenta);
                    //_stringType = "Elite";
                    _stringType = "Combat";
                    break;
                }

        }

        _RoomIcon3D.GetComponent<MeshRenderer>().material.SetColor("_Primary_Color", _defaultHoloColor);
        _RoomIcon3D.GetComponent<MeshRenderer>().material.SetColor("_Fresnel_Color", _defaultFresnelColor);
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
        //GetComponent<MeshRenderer>().material.color = Color.grey;
        _RoomIcon3D.GetComponent<MeshRenderer>().material.SetColor("_Secondary_Color", new Color(0.209226f, 0.3180681f, 0.509838f));
        //_RoomIcon3D.GetComponent<MeshRenderer>().material.SetColor("_Fresnel_Color", new Color(0.114f, 0.114f, 0.114f));
        _RoomIcon3D.GetComponent<RoomIconAnim>().Lock();

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
        _RoomIcon3D.GetComponent<RoomIconAnim>().MouseEnter();
    }

    private void OnMouseExit()
    {
        _RoomIcon3D.GetComponent<RoomIconAnim>().MouseExit();
    }

    public void Contaminate(bool value)
    {
        switch (value)
        {
            case true:
                Outline outline = gameObject.AddComponent<Outline>();
                outline.OutlineColor = new Color(91, 255, 0);
                outline.OutlineWidth = 5;
                break;

            case false:
                if(gameObject.TryGetComponent<Outline>(out Outline target))
                {
                    Destroy(target);
                }
                break;
        }
    }
}
