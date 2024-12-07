using System;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public GameObject _cardPrefab;
    public GameObject _cardRowPrefab;
    public Transform _cardRowParent;
    public GameObject _panel;
    public Canvas _canvas;
    private GameObject _cardClone;

    private void Start()
    {
        // Add a Button component to the panel if it doesn't already have one
        Button panelButton = _panel.GetComponent<Button>();
        if (panelButton == null)
        {
            panelButton = _panel.AddComponent<Button>();
        }

        // Add the CloseZoom method to the panel's Button onClick event
        panelButton.onClick.AddListener(CloseZoom);
    }

    public void OnCardClick(GameObject card)
    {
        if (_panel == null)
        {
            Debug.LogError("Panel is not assigned.");
            return;
        }

        if (_canvas == null)
        {
            Debug.LogError("Canvas is not assigned.");
            return;
        }

        if (_cardClone != null)
        {
            Destroy(_cardClone);
        }

        _panel.SetActive(true);

        // Create a clone of the card
        _cardClone = Instantiate(card, _panel.transform);

        // Remove the Button component from the clone to avoid multiple listeners
        Button cloneButton = _cardClone.GetComponent<Button>();
        if (cloneButton != null)
        {
            Destroy(cloneButton);
        }

        // Set the clone's transform properties to center it on the panel
        RectTransform cardRect = _cardClone.GetComponent<RectTransform>();
        RectTransform panelRect = _panel.GetComponent<RectTransform>();

        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = Vector2.zero;
        //cardRect.sizeDelta = new Vector2(Mathf.Abs(panelRect.sizeDelta.x), Mathf.Abs(panelRect.sizeDelta.y) + 416); // Adjust to keep the aspect ratio

        // Scale the card clone and its children
        _cardClone.transform.localScale = new Vector3(2, 2, 2);

        // Recursively scale all children
        ScaleChildren(_cardClone.transform, new Vector3(2, 2, 2));

        // Bring the panel to the front
        _panel.transform.SetAsLastSibling();
    }

    private void ScaleChildren(Transform parent, Vector3 scale)
    {
        foreach (Transform child in parent)
        {
            child.localScale = scale;
            ScaleChildren(child, scale); // Recursively scale all children
        }
    }

    public void CloseZoom()
    {
        if (_cardClone != null)
        {
            Destroy(_cardClone);
        }

        _panel.SetActive(false);
    }
}
