using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidBackPlate : MonoBehaviour
{
    bool _disabling;

    void OnMouseEnter()
    {
        if(GI._PManFetcher()._currentState == "movement")
        {
            GI._PManFetcher().SetToState("Empty");
            _disabling = true;
        }
    }

    private void OnMouseExit()
    {
        if(_disabling)
            GI._PManFetcher().SetToLastState();

        _disabling=false;
    }
}
