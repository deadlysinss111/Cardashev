using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutScript : MonoBehaviour
{
    public void TriggerAnimOver()
    {
        GameObject.Find("Menu").GetComponent<MainMenuManager>().StartFadeOutOver();
    }
}
