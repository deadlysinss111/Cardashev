using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    /*
     FIELDS
    */
    // Card identity
    // TODO make them into protected with properties
    public string _name;
    public string _description;
    public float _duration;
    public Dictionary<string, int> _stats;
    public int _goldValue;

    // Utilities
    public float _cardEndTimestamp;
    public byte _id = 0;
    protected SelectableArea _selectableArea;
    protected RotationSelectArrow _rotationArrow;
    protected LineRenderer _lineRenderer;
    protected Vector3 _lastScale;
    protected Vector3 _lastPos;
    protected Action _timeStopedEvent = ()=> { };
    protected Action _timeStopedClick = ()=> { };
    [NonSerialized] public bool _isCollectible;

    [SerializeField] protected LayerMask _clickableLayers;
    public Color _actionColor;

    // Actions
    public Action _onDiscard;       // Called when the card's duration reached 0 after activation
    public Action _trigger;         // Called when the Queue sets it to be active
    public Action _clickEffect;     // Called when the card is clicked in the HUD

    // Used for previews
    [SerializeField] protected bool _shotForPreview;// = false;
    [SerializeField] protected bool _trajectoryForPreview = false;
    protected UnityEngine.Object _ghostHitboxPrefab;
    protected GameObject _ghostHitbox = null;
    protected Vector3 _velocityFromLastBellCurveCalculated;
    protected Vector3 _originFromLastBellCurveCalculated;
    protected Vector3 _destinationFromLastBellCurveCalculated;
    [SerializeField] protected float _apex = 10.0f;
    bool isMovementCard = false;

    // Level related
    public int _currLv;
    public int _maxLv;

    public enum CollectibleState
    {
        NOTHING = 0,
        ADDTODECK,
        BACKTOPLAYABLE,
        ADDTODECKANDBACKTOPLAY
    }

    public enum PreviewZoneType
    {
        NONE = -1,
        SPHERE = 0,
        ELLIPSIS,
    }

    [NonSerialized] public GameObject _target;

    /*
     METHODS
    */
    public Card()
    {
        _currLv = 1;
        _maxLv = 3;
        _trigger += () => Effect();
        _clickEffect = PlayCard;
        _isCollectible = false;
    }

    protected void Init(float duration, byte maxLvl, int goldValue, Dictionary<string, int> stats, string description = "", PreviewZoneType previewType = PreviewZoneType.NONE)
    {
        _timeStopedEvent = TimeStopedMouseEnter;

        _duration = duration;
        _maxLv = maxLvl;
        _goldValue = goldValue;
        _stats = stats;
        _description = description;

        if(TryGetComponent(out LineRenderer renderer))
            _lineRenderer = renderer;
        else
            _lineRenderer = null;

        switch(previewType)
        {
            case PreviewZoneType.NONE:
                _ghostHitboxPrefab = Resources.Load("NullPreview");
                break;
            case PreviewZoneType.SPHERE:
                _ghostHitboxPrefab = Resources.Load("SpherePreview");
                break;
            case PreviewZoneType.ELLIPSIS:
                _ghostHitboxPrefab = Resources.Load("EllipsisPreview");
                break;
            default:
                Debug.LogError("error in preview type for your card : " + _name);
                break;
        }
    }

    // ~~~(> GETTERS
    public float GetRemainingTime()
    {
        return _cardEndTimestamp - Time.time;
    }

    // In shop & rewards behaviour
    // TODO => move it to its own, new class
    // Set a card as collectible, assign it a function that will run on click, and determine what to do with the card depending on the result (nothin / add to deck...)
    public void SetToCollectible(Func<CollectibleState> func)
    {
        _isCollectible = true;
        _clickEffect = () =>
        {
            switch (func())
            {
                case CollectibleState.NOTHING:
                    return;
                case CollectibleState.ADDTODECK:
                    CurrentRunInformations.AddCardsToDeck(new List<GameObject> { gameObject });
                    _isCollectible = false;
                    break;
                case CollectibleState.BACKTOPLAYABLE:
                    _clickEffect = PlayCard;
                    _isCollectible = false;
                    break;
                case CollectibleState.ADDTODECKANDBACKTOPLAY:
                    print("bah je suis la bouffon");
                    CurrentRunInformations.AddCardsToDeck(new List<GameObject> { gameObject });
                    _clickEffect = PlayCard;
                    _isCollectible = false;
                    break;
            };
        };
    }

    public void SetToPlayable()
    {
        _clickEffect = PlayCard;
        _isCollectible = false;
    }


    // -----
    // ACTIONS, FUNCS AND CALLBACKS
    // -----
    public virtual void Effect()
    {
        _cardEndTimestamp = Time.time + _duration;
        
        if(_ghostHitbox != null)
            Destroy(_ghostHitbox);
    }

    // Need that stuff to make the card interract as if the time wasn't stop when it is //
    // All of this is wrecking the perfs //
    void Update()
    {
        if (Time.timeScale == 0)
        {
            _timeStopedEvent();
            if (Input.GetMouseButtonDown(0))
            {
                GI._changeStateOnHUDExit = false;
                _timeStopedClick();
            }
        }
    }

    void TimeStopedMouseEnter()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        GameObject.Find("Canvas").GetComponent<GraphicRaycaster>().Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            GameObject cardObj = HierarchySearcher.FindParentdRecursively(result.gameObject.transform, gameObject.name);
            if (cardObj != null)
            {
                if (cardObj.GetComponent<Card>()._id != _id) continue;
                OnMouseEnter();
                _timeStopedEvent = TimeStopedMouseExit;
                _timeStopedClick = ()=> { _clickEffect(); GI.temp = false; };
                break;
            }
        }
    }


    void TimeStopedMouseExit()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        GameObject.Find("Canvas").GetComponent<GraphicRaycaster>().Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            GameObject cardObj = HierarchySearcher.FindParentdRecursively(result.gameObject.transform, gameObject.name);
            if (cardObj != null)
            {
                if (cardObj.GetComponent<Card>()._id != _id) continue;
                return;
            }
        }
        OnMouseExit();
        _timeStopedEvent = TimeStopedMouseEnter;
        _timeStopedClick = () => { };
    }

    

    // Regular way to interract with cards

    // make the card bigger when mouse is over it
    void OnMouseEnter()
    {
        if (false == _isCollectible)
        {
            _lastScale = transform.localScale;
            _lastPos = transform.localPosition;
            transform.localScale *= 2;
            transform.localPosition += new Vector3(0, 200, 0);
        }
        else
        {
            _lastScale = transform.localScale;
            transform.localScale *= 1.4f;
        }
            
    }
    void OnMouseExit()
    {
        if (false == _isCollectible)
        {
            transform.localScale = _lastScale;
            transform.localPosition = _lastPos;
        }
        else
        {
            transform.localScale = _lastScale;
        }
    }


    void OnMouseDown()
    {
        _clickEffect();
    }

    public virtual void PlayCard()
    {
        GI._PlayerFetcher().GetComponent<DeckManager>().Play(this);
    }

    protected void ClearPath()
    {
        _lineRenderer.positionCount = 0;
    }


    // ------
    // UPGRADE METHODS
    // ------
    public virtual void OnUpgrade()
    {
       
    }

    public bool CanUpgrade()
    {
        return _currLv < _maxLv;
    }

    public bool Upgrade()
    {
        if (CanUpgrade())
        {
            _currLv++;
            GameObject frame = (GameObject)Instantiate(Resources.Load("lvl" + _currLv + "sprite"), gameObject.transform);
            print("frame : " + frame);
            OnUpgrade();
            UpdateDescription();
            return true;
        }
        return false;
    }

    public void UpdateDescription()
    {
        HierarchySearcher.FindChildRecursively(transform, "Text").GetComponent<TextMeshProUGUI>().SetText(_description);
        HierarchySearcher.FindChildRecursively(transform, "Duration").GetComponent<TextMeshProUGUI>().SetText(_duration.ToString());
    }

    static public GameObject Instantiate(string name, bool isActive = false)
    {
        GameObject card = Instantiate((GameObject)Resources.Load(name));
        card.layer = LayerMask.NameToLayer("UI");
        card.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        card.GetComponent<Card>().UpdateDescription();
        card.SetActive(isActive);

        return card;
    }

    // Called when the card is loaded / unloaded by the DeckManager
    public virtual void OnLoad() { }

    public virtual void OnUnload() { }

    // ------
    // METHODS TO HANDLE GHOSTS / PREVIEW
    // ------
    protected void Preview()
    {
        // Useful for preview positioning
        Vector3 selTilePos = GI._PManFetcher()._lastHit.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        Vector3 shoulderOffset = new Vector3(0, 1, 0);

        bool shouldCullPreview = false;

        // For some reason duplciating this code breaks some culling actions lmao
        if (_selectableArea != null)
            shouldCullPreview = !_selectableArea.CheckForSelectableTile(selTilePos);

        // Shows the ghost hitbox at selected tile
        if (_ghostHitboxPrefab != null)
        {
            // First instantiation
            if (_ghostHitbox == null)
            {
                _ghostHitbox = Instantiate((GameObject)_ghostHitboxPrefab);
                if(_selectableArea != null)
                    EnsureSelectableTilesByRain();
            }

            // Makes the preview dissapear if the play would be invalid
            if (shouldCullPreview)
                _ghostHitbox.SetActive(false);

            // Updates and shows the ghost hitbox
            else
            {
                _ghostHitbox.transform.position = selTilePos;
                _ghostHitbox.SetActive(true);
            }
        }

        // Shows the ghost ray at selected tile
        if (_shotForPreview)
        {
            if (shouldCullPreview)
                ClearPath();
            if (_ghostHitbox != null)
                TrajectoryToolbox.LineRenderWithTarget(GI._PManFetcher()._virtualPos + new Vector3(0, 1.8f, 0), selTilePos + new Vector3(0, 1.5f, 0), ref _ghostHitbox.GetComponent<AOEPreviewScript>()._lineRenderer);
        }

        // Show the trajectory at selected tile
        if (_trajectoryForPreview)
        {
            // Makes the preview dissapear if the play would be invalid
            if (shouldCullPreview)
                ClearPath();

            //// Updates and shows the trajectory
            //Vector3 curveOrigin = GI._PManFetcher()._virtualPos;
            //Vector3 curveInitVelocity = TrajectoryToolbox.BellCurveInitialVelocity(curveOrigin + shoulderOffset, selTilePos, 10.0f);
            //TrajectoryToolbox.BellCurve(curveOrigin, curveInitVelocity, ref _lineRenderer);

            _originFromLastBellCurveCalculated = GI._PManFetcher()._virtualPos;
            _velocityFromLastBellCurveCalculated = TrajectoryToolbox.BellCurveInitialVelocity(_originFromLastBellCurveCalculated + new Vector3(0, 1, 0), selTilePos + new Vector3(0, 1, 0), _apex);
            _destinationFromLastBellCurveCalculated = selTilePos;

            if (_ghostHitbox != null)
                TrajectoryToolbox.BellCurve(_originFromLastBellCurveCalculated + new Vector3(0, 1, 0), _velocityFromLastBellCurveCalculated, ref _ghostHitbox.GetComponent<AOEPreviewScript>()._lineRenderer);
        }
    }

    protected void EnsureSelectableTilesByRain()
    {
        List<GameObject> tiles = _selectableArea.GetSelectableTiles();
        foreach(GameObject tile in tiles)
        {
            if(TrajectoryToolbox.RaycastBellCurve(GI._PManFetcher()._virtualPos + new Vector3(0, 1, 0), tile.transform.position + new Vector3(0, 1, 0), _apex).gameObject != tile)
            {
                tile.GetComponent<Tile>().SetSelected(false);
            }
        }
    }

    public void DestroyPreview()
    {
        throw new NotImplementedException();
    }
}