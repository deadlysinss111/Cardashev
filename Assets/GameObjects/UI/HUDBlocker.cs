using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HUDBlocker : MonoBehaviour
{
    [SerializeField] GameObject that;
    Action _timeStopedEvent = () => { };

    private void Start()
    {
        _timeStopedEvent = TimeStopedMouseEnter;
    }

    void Update()
    {
        if (Time.timeScale == 0)
            _timeStopedEvent();
        else
        {
            Destroy(that);
            Destroy(this);
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
                GI._PManFetcher().SetToState("Empty");
                GI.temp = true;
                _timeStopedEvent = TimeStopedMouseExit;
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
        if (GI._changeStateOnHUDExit)
        {
            GI._PManFetcher().SetToLastState();
            //GI.temp = false;
            _timeStopedEvent = TimeStopedMouseEnter;
        }
    }
}
