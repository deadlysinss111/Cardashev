using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public static class CardCollection
{
    static int _collectionMaxCapacity = 200;
    static public List<GameObject> _collection = new();

    static public GameObject _cardCollection;

    static public GameObject _rowPrefab;

    static public CardManager _cardManager;

    static public int _cardsPerRow = 6;

    // Method to add cards to the collection
    static public void AddCardsToCollection(List<string> cardNames)
    {
        _cardCollection = GameObject.Find("CardsCollection");
        _rowPrefab = Resources.Load<GameObject>("CardRowy");
        _cardManager = GameObject.Find("Menu").GetComponent<CardManager>();

        int cardIndex = 0;
        GameObject currentRow = null;

        if (cardNames.Count + _collection.Count > _collectionMaxCapacity)
        {
            Debug.LogError("_Collection was passed with a List too large ! ALL CARDS REFUSED");
        }
        else
        {
            foreach (string name in cardNames)
            {
                if (cardIndex % _cardsPerRow == 0)
                {
                    currentRow = MonoBehaviour.Instantiate(_rowPrefab, _cardCollection.transform);
                }

                GameObject card = Card.Instantiate(name);
                if (card != null)
                {
                    _collection.Add(card);
                    card.transform.SetParent(currentRow.transform);
                    card.transform.localPosition = Vector3.zero;
                    card.transform.localScale = new Vector3(2,2,2);
                    card.GetComponent<RectTransform>().sizeDelta = new Vector3(100, 150, 0);
                    card.GetComponent<BoxCollider>().enabled = false;
                    card.GetComponent<LineRenderer>().enabled = false;
                    card.gameObject.SetActive(true);
                    MonoBehaviour[] scripts = card.GetComponents<MonoBehaviour>();
                    foreach (MonoBehaviour script in scripts)
                    {
                        script.enabled = false;
                    }

                    Button button = card.AddComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => _cardManager.OnCardClick(card));
                    }
                    else
                    {
                        Debug.LogError("Button component is not found on the card prefab.");
                    }

                    cardIndex++;
                }
                else
                {
                    Debug.LogError("Failed to instantiate card: " + name);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(_cardCollection.GetComponent<RectTransform>());
            }
        }
    }

    // Method to get the current collection
    static public List<GameObject> GetCollection()
    {
        return _collection;
    }
}
