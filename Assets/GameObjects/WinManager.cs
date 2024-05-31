using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    public bool _onWinScreen = false;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        //if (gameObject.activeInHierarchy == false)
        _onWinScreen=true;
    }
}
