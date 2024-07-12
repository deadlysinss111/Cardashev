using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotationSelectArrow : MonoBehaviour
{
    GameObject _arrowCanvas;
    Image _arrow;

    // Start is called before the first frame update
    void Start()
    {
        if (GI._PlayerFetcher().transform.Find("arrowCanvas"))
            return;
        // Creates a world canvas around the player
        _arrowCanvas = new("arrowCanvas");
        _arrowCanvas.transform.SetParent(GI._PlayerFetcher().transform, false);
        Canvas canvas = _arrowCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        RectTransform rt = _arrowCanvas.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(0, 0.5f, 0);
        rt.sizeDelta = new Vector2(1, 1);
        rt.localScale = new Vector3(3, 3, 3);

        _arrowCanvas.AddComponent<CanvasScaler>();
        _arrowCanvas.AddComponent<GraphicRaycaster>();

        // Creates the image of the arrow on the canvas around the player
        GameObject arrowObj = new("arrowImage");
        arrowObj.transform.SetParent(_arrowCanvas.transform, false);
        Texture2D tx = Resources.Load("arrow") as Texture2D;
        _arrow = arrowObj.AddComponent<Image>();
        _arrow.sprite = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(0.5f, 0.5f));
        _arrow.raycastTarget = false;
        rt = _arrow.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(0, 0.2f, 0.5f);
        rt.sizeDelta = new Vector2(10f, 42f);
        rt.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        rt.localRotation = Quaternion.Euler(90f, 0f, 0f);

        // Sets their layer to WorldUI, which is ignored by the middleware
        _arrowCanvas.layer = LayerMask.NameToLayer("WorldUI");
        arrowObj.layer = LayerMask.NameToLayer("WorldUI");

        _arrowCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_arrowCanvas.activeSelf)
        {
            _arrowCanvas.transform.position = GI._PManFetcher()._virtualPos;
            Vector3 mPos = Input.mousePosition;
            mPos.z = Camera.main.WorldToScreenPoint(GI._PManFetcher()._virtualPos).z;
            //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity) == false) return;
            Vector3 wPos = Camera.main.ScreenToWorldPoint(mPos);
            print("PosMouse: "+wPos);

            //BetterDebug.Log(Input.mousePosition, _arrowCanvas.transform.rotation);
            Quaternion rot = Quaternion.LookRotation(wPos - GI._PManFetcher()._virtualPos);
            rot.eulerAngles = new Vector3(0, rot.eulerAngles.y, rot.eulerAngles.z);

            _arrowCanvas.transform.rotation = rot;
        }
    }

    public void SetArrow(bool value)
    {
        _arrowCanvas.SetActive(value);
    }

    public Quaternion GetRotation()
    {
        return _arrowCanvas.transform.rotation;
    }
}
