using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterManager : MonoBehaviour
{
    public CharacterDatabase _characterDatabase;
    public SpriteRenderer _spriteRenderer;
    public CollectionMenu _collection;

    private int _selectedOption = 0;
    private GameObject _prevChar;
    private GameObject _nextChar;

    private void Start()
    {
        _prevChar = GameObject.Find("PreviousCharacter");
        _nextChar = GameObject.Find("NextCharacter");

        // Initialize the selected option indices
        UpdateCharacter(_selectedOption);
        UpdateAdjacentCharacters();
    }

    // Change the character to the next one
    public void NextOption()
    {
        _selectedOption = (_selectedOption + 1) % _characterDatabase.count;
        UpdateCharacter(_selectedOption);
        UpdateAdjacentCharacters();
    }

    // Change the character to the previous one
    public void BackOption()
    {
        _selectedOption = (_selectedOption - 1 + _characterDatabase.count) % _characterDatabase.count;
        UpdateCharacter(_selectedOption);
        UpdateAdjacentCharacters();
    }

    // Update the character sprite and display the collection
    private void UpdateCharacter(int selectedOption)
    {
        CharacterForDB character = _characterDatabase.GetCharacter(selectedOption);
        _spriteRenderer.sprite = character._characterSprite;

        if (_collection == null)
        {
            Debug.LogError("Collection reference is not assigned in CharacterManager.");
        }
        else
        {
            _collection.DisplayCollection(character._characterName);
        }
    }

    private void UpdateAdjacentCharacters()
    {
        int selectedOptionPrev = (_selectedOption - 1 + _characterDatabase.count) % _characterDatabase.count;
        int selectedOptionNext = (_selectedOption + 1) % _characterDatabase.count;

        UpdatePrevChar(selectedOptionPrev);
        UpdateNextCharacter(selectedOptionNext);
    }

    private void UpdatePrevChar(int selectedOptionPrev)
    {
        CharacterForDB previous = _characterDatabase.GetCharacter(selectedOptionPrev);
        _prevChar.GetComponent<SpriteRenderer>().sprite = previous._characterSprite;

        if (previous == null)
        {
            Debug.LogError("Previous character not found.");
        }
    }

    private void UpdateNextCharacter(int selectedOptionNext)
    {
        CharacterForDB next = _characterDatabase.GetCharacter(selectedOptionNext);
        _nextChar.GetComponent<SpriteRenderer>().sprite = next._characterSprite;

        if (next == null)
        {
            Debug.LogError("Next character not found.");
        }
    }
}