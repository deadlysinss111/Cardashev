using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    public GameObject _shopInterface { get; private set; }

    private void Awake()
    {
        Transform[] childer = GetComponentsInChildren<Transform>();
        foreach (Transform child in childer)
        {
            if (child.gameObject.name == "ShopInterface")
            {
                _shopInterface = child.gameObject;
                _shopInterface.SetActive(false);
                break;
            }
        }
    }
}
