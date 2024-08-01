using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionButton : MenuButton
{
    private void OnMouseDown()
    {
        GameObject.Find("Menu").GetComponent<MainMenuManager>().GoToCollection();
        base.OnMouseExit();
    }
}
