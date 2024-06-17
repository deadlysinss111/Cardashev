using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionMenu: MonoBehaviour
{
    private GameObject _cardsContainer;
    private List<GameObject> _currentUnlocked;
    private List<GameObject> _currentLocked;

    public GameObject _cardRowPrefab;
    public GameObject _cardPrefab;
    public CardManager _cardManager;

    private void Awake()
    {
        _cardsContainer = GameObject.Find("CardsCollection");
    }

    // Unlocked cards for each player
    public static Dictionary<string, List<string>> _unlocked = new Dictionary<string, List<string>>()
    {
        { "Todd", new List<string>(){ "Geralt", "Ciri", "Yennefer", "Triss", "Keira_Metz", "Gaunt_O_Dimm", "Philippa_Eilhart" } },
        //{ "Jack", new List<string>(){ "CardModel" } },
    };

    // Locked cards for each player
    public static Dictionary<string, List<string>> _locked = new Dictionary<string, List<string>>()
    {
        { "Todd", new List<string>(){ "Gaunt_O_Dimm", "Philippa_Eilhart" } },
        //{ "Jack", new List<string>(){ "CardModel" } },
    };

    // Load the cards for the player that fits the given name
    private void Load(string playerName)
    {
        _currentUnlocked = new List<GameObject>();
        _currentLocked = new List<GameObject>();

        if (_unlocked.TryGetValue(playerName, out List<string> unlockedPool))
        {
            AddCardsToContainer(unlockedPool, _currentUnlocked, false);
        }

        if (_locked.TryGetValue(playerName, out List<string> lockedPool))
        {
            AddCardsToContainer(lockedPool, _currentLocked, true);
        }
    }

    // Add the cards to the container
    private void AddCardsToContainer(List<string> pool, List<GameObject> targetList, bool locked)
    {
        byte cardsPerRow = 6;

        GameObject currentRow = null;

        for (int i = 0; i < pool.Count; i++)
        {
            if (i % cardsPerRow == 0)
            {
                currentRow = Instantiate(_cardRowPrefab, _cardsContainer.transform);
            }

            string spriteResourcePath = pool[i];
            Sprite cardSprite = Resources.Load<Sprite>(spriteResourcePath);

            if (cardSprite != null)
            {
                GameObject cardObj = Instantiate(_cardPrefab, currentRow.transform);
                Image cardImage = cardObj.GetComponent<Image>();

                if (cardImage != null)
                {
                    cardImage.sprite = cardSprite;

                    // Gray out the card if it's locked
                    if (locked)
                    {
                        cardImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Adjust alpha as needed
                        cardObj.GetComponent<Button>().interactable = false; // Make it non-interactable
                    }
                    else
                    {
                        cardImage.color = Color.white; // Reset color if unlocked
                        cardObj.GetComponent<Button>().interactable = true; // Make it interactable
                    }
                }

                Debug.Log("Card created with sprite: " + spriteResourcePath);
                targetList.Add(cardObj);

                // Add the OnCardClick method to the button component of the card
                Button button = cardObj.GetComponent<Button>();
                if (button != null && !locked) // Only add listener if card is unlocked
                {
                    button.onClick.AddListener(() => _cardManager.OnCardClick(cardObj));
                }
                else if (button == null)
                {
                    Debug.LogError("Button component is not found on the card prefab.");
                }
            }
            else
            {
                Debug.LogError("Sprite not found in Resources: " + spriteResourcePath);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_cardsContainer.GetComponent<RectTransform>());
    }

    // Display the collection for the given player
    public void DisplayCollection(string playerName)
    {
        Debug.Log("Displaying collection for player: " + playerName);

        foreach (Transform child in _cardsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        Load(playerName);

        _currentUnlocked.Clear();
        _currentLocked.Clear();
    }
}