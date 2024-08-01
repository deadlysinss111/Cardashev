using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGame : MenuButton
{
    private void OnMouseDown()
    {
        GameObject.Find("Menu").GetComponent<MainMenuManager>().GoToCharacterSelection();
        base.OnMouseExit();
    }
}
