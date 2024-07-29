using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckDisplay : MonoBehaviour
{
    GameObject _deckContainer;
    public GameObject _cardsCollection;

    GameObject _collection;

    public GameObject _cardRowPrefab; // Prefab for a row of cards
    public int cardsPerRow = 6; // Number of cards per row

    private void Awake()
    {
        _deckContainer = GameObject.Find("DeckContainer");

        _collection = GameObject.Find("Collection");
        _collection?.SetActive(false);
    }

    public void Click()
    {
        _collection?.SetActive(true);
        CloneAndDisableScripts();
    }

    void CloneAndDisableScripts()
    {
        int cardIndex = 0;
        GameObject currentRow = null;

        foreach (Transform card in _deckContainer.transform)
        {
            if (cardIndex % cardsPerRow == 0)
            {
                currentRow = Instantiate(_cardRowPrefab, _cardsCollection.transform);
            }

            // Clone the card
            GameObject clonedCard = Instantiate(card.gameObject, currentRow.transform);
            clonedCard.SetActive(true);
            clonedCard.transform.localScale = new Vector3(2,2,2);
            clonedCard.transform.localPosition = Vector3.zero;

            // Disable all scripts on the cloned card
            MonoBehaviour[] scripts = clonedCard.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = false;
            }

            cardIndex++;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_cardsCollection.GetComponent<RectTransform>());
    }

    public void Exit()
    {
        // Destroy all cloned cards in _cardsCollection
        foreach (Transform card in _cardsCollection.transform)
        {
            Destroy(card.gameObject);
        }

        _collection.SetActive(false);
    }
}
