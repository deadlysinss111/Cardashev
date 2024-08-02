using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MenuButton
{
    private void OnMouseDown()
    {
        GameObject.Find("Menu").GetComponent<MainMenuManager>().QuitGame();
        base.OnMouseExit();
    }
}
