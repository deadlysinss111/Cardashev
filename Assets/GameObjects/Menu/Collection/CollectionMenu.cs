using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionMenu : MonoBehaviour
{
    public GameObject _cardRowPrefab; // Prefab for a row of cards
    public int cardsPerRow = 6; // Panel containing the collection menu UI elements
    public CardManager _cardManager;

    /*    private void Awake()
        {
            _cardsContainer = GameObject.Find("CardsCollection");
            if (collectionMenuPanel != null)
            {
                collectionMenuPanel.SetActive(false); // Initially hide the panel
            }
        }

        private void AddCardsToContainer(List<GameObject> cards)
        {
            byte cardsPerRow = 6;
            GameObject currentRow = null;

            for (int i = 0; i < cards.Count; i++)
            {
                if (i % cardsPerRow == 0)
                {
                    currentRow = Instantiate(_cardRowPrefab, _cardsContainer.transform);
                }

                GameObject cardObj = cards[i];

                if (cardObj != null)
                {
                    cardObj.transform.SetParent(currentRow.transform);

                    Button button = cardObj.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => _cardManager.OnCardClick(cardObj));
                    }
                    else
                    {
                        Debug.LogError("Button component is not found on the card prefab.");
                    }
                }
                else
                {
                    Debug.LogError("Card object is null at index: " + i);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_cardsContainer.GetComponent<RectTransform>());
        }

        public void DisplayCollection()
        {
            Debug.Log("Displaying collection");

            foreach (Transform child in _cardsContainer.transform)
            {
                Destroy(child.gameObject);
            }

            List<GameObject> collection = CardCollection.GetCollection();
            AddCardsToContainer(collection);
        }*/



    // Method to toggle the collection menu
    public void ToggleCollectionMenu(string name)
    {
        Idealist.CollectionDeck(name);
    }
}
