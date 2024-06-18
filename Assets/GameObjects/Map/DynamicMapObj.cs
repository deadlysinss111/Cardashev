using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class DynamicMapObj : MonoBehaviour
{
    /*
     METHODS
    */
    protected abstract void UpdDynamicMapObj();

    protected private void Awake()
    {
        // Collective event subscribing
        GI._UeOnMapSceneLoad.AddListener(UpdDynamicMapObj);
    }
}
