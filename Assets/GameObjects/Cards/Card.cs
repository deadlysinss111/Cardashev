using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
    public int[] _stats;
    public int _goldValue;

    // Utilities
    public float _cardEndTimestamp;
    public byte _id = 0;
    protected SelectableArea _selectableArea;
    protected LineRenderer _lineRenderer;
    protected Vector3 _lastScale;
    protected Action _timeStopedEvent = ()=> { };
    protected Action _timeStopedClick = ()=> { };

    [SerializeField] protected LayerMask _clickableLayers;
    public Color _actionColor;

    // Actions
    public Action _onDiscard;       // Called when the card's duration reached 0 after activation
    public Action _trigger;         // Called when the Queue sets it to be active
    public Action _clickEffect;     // Called when the card is clicked in the HUD

    // Level related
    public int _currLv;
    public int _maxLv;

    /*
     METHODS
    */
    public Card()
    {
        _currLv = 1;
        _maxLv = 3;
        _trigger += () => Effect();
        _clickEffect = PlayCard;
    }

    protected void Init(float duration, byte maxLvl, int goldValue, int[] stats, string description = "")
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
    }

    // ~~~(> GETTERS
    public float GetRemainingTime()
    {
        return _cardEndTimestamp - Time.time;
    }

    // In shop & rewards behaviour
    // TODO => move it to its own, new class
    public void SetToCollectible(Func<bool> func)
    {
        _clickEffect = () => { if (func()) CurrentRunInformations.AddCardsToDeck(new List<string> { _name }); };
    }


    // -----
    // ACTIONS, FUNCS AND CALLBACKS
    // -----
    public virtual void Effect()
    {
        _cardEndTimestamp = Time.time + _duration;
    }
    public virtual void Effect(GameObject go)
    {
        Effect();
    }

    // Need that stuff to make the card interract as if the time wasn't stop when it is //
    // All of this is wrecking the perfs //
    void Update()
    {
        if (Time.timeScale == 0)
        {
            _timeStopedEvent();
            if(Input.GetMouseButtonDown(0))
                _timeStopedClick();
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
            if (HierarchySearcher.FindParentdRecursively(result.gameObject.transform, gameObject.name))
            {
                OnMouseEnter();
                _timeStopedEvent = TimeStopedMouseExit;
                _timeStopedClick = _clickEffect;
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
            if (HierarchySearcher.FindParentdRecursively(result.gameObject.transform, gameObject.name))
            {
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
        _lastScale = transform.localScale;
        transform.localScale *= 2;
        transform.localPosition += new Vector3(0, 200, 0);
    }
    void OnMouseExit()
    {
        transform.localScale = _lastScale;
        transform.localPosition -= new Vector3(0, 200, 0);
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
        for (int i = 0; i < _stats.Length; i++)
        {
            _stats[i] += 2 * _currLv;
        }
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
            GameObject frame = (GameObject)Instantiate(Resources.Load("lvl" + _currLv+"sprite"), gameObject.transform);
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

    static public GameObject Instantiate(string name)
    {
        GameObject card = Instantiate((GameObject)Resources.Load(name));
        card.layer = LayerMask.NameToLayer("UI");
        card.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        card.GetComponent<Card>().UpdateDescription();
        card.SetActive(false);

        return card;
    }

    // Called when the card is loaded / unloaded by the DeckManager
    public virtual void OnLoad() { }

    public virtual void OnUnload() { }
}