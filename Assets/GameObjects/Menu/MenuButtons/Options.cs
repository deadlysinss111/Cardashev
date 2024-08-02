using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MenuButton
{
    private void OnMouseDown()
    {
        GameObject.Find("Menu").GetComponent<MainMenuManager>().GoToOptions();
        base.OnMouseExit();
    }
}
