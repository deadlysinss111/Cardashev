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
        _arrowCanvas = new("arrowCanvas");
        _arrowCanvas.transform.SetParent(GI._PlayerFetcher().transform);
        Canvas canvas = _arrowCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        RectTransform rt = _arrowCanvas.GetComponent<RectTransform>();
        rt.position = new Vector3(0, 1.5f, 0);
        rt.sizeDelta = new Vector2(1, 1);
        rt.localScale = new Vector3(3, 3, 3);

        _arrowCanvas.AddComponent<CanvasScaler>();
        _arrowCanvas.AddComponent<GraphicRaycaster>();


        GameObject arrowObj = new("arrowImage");
        arrowObj.transform.SetParent(_arrowCanvas.transform, false);
        Texture2D tx = Resources.Load("arrow") as Texture2D;
        _arrow = arrowObj.AddComponent<Image>();
        _arrow.sprite = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(0.5f, 0.5f));
        rt = _arrow.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(0, 0.2f, 1.1f);
        rt.sizeDelta = new Vector2(10f, 42f);
        rt.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        rt.localRotation = Quaternion.Euler(90f, 0f, 0f);

        _arrowCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_arrowCanvas.activeSelf)
        {
            Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Quaternion rot = Quaternion.LookRotation(mPos - GI._PlayerFetcher().transform.position);
            rot.eulerAngles = new Vector3(0, rot.eulerAngles.y, rot.eulerAngles.z);

            _arrowCanvas.transform.rotation = Quaternion.Lerp(rot, _arrowCanvas.transform.rotation, 0);
        }
    }

    public void SetArrow(bool value)
    {
        _arrowCanvas.SetActive(value);
    }

    public Quaternion ReturnArrowRotation()
    {
        return _arrowCanvas.transform.rotation;
    }
}
