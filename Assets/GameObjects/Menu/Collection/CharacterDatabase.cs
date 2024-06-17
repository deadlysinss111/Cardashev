using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


// allows the creation of a new CharacterDatabase asset
[CreateAssetMenu]

public class CharacterDatabase : ScriptableObject
{
    public CharacterForDB[] _character;

    // returns the number of characters in the database
    public int count
    {
        get
        {
            return _character.Length;
        }
    }

    // returns the character at the given index
    public CharacterForDB GetCharacter(int index)
    {
        return _character[index];
    }

}