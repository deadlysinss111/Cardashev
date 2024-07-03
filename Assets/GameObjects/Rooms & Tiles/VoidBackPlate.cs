using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBackPlate : MonoBehaviour
{
    void OnMouseEnter()
    {
        GI._PManFetcher().SetToState("Empty");
    }

    private void OnMouseExit()
    {
        GI._PManFetcher().SetToLastState();
    }
}
