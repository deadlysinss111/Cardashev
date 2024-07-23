using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplicitInteractions : MonoBehaviour
{
    /*
     FIELDS 
    */
    // Interactible the player selects
    GameObject _currentInteractible;
    PlayerManager _manager;

    /*
     METHODS
    */
    private void Awake()
    {
        // Loading in PlayerManager a new state and its Action to change what the controls will do
        _manager = GI._PManFetcher();
        PlayerManager.AddOrOverrideState("InteractibleTargeting", OnEnterState, OnExitState);
    }

    // Pair of Action for State changes
    private void OnEnterState()
    {
        // Sets up the functions for the controls
        _manager.SetLeftClickTo(RaycastResponseCaller);
        _manager.SetHoverTo( () => { } );
        _manager.SetRightClickTo( () => { } );
    }
    private void OnExitState() {  }


    // Calls the response to a raycast hit event of the Interactbile target
    public void RaycastResponseCaller()
    {
        // Updates the current Interactible
        //_currentInteractible = FindPrefabOriginRecur(_manager._lastHit.collider.gameObject);
        if (_manager._lastHit.transform == null) return;
        _currentInteractible = FindPrefabOriginRecur(_manager._lastHit.transform.gameObject);
        //print("target is : "+ _currentInteractible.name);
        print(_currentInteractible);

        // "Raises" a RaycastHit event, given the Interactible isn't null for some reason
        if (_currentInteractible != null)
            _currentInteractible.GetComponent<Interactible>().OnRaycastHit();
    }


    // ------
    // UTILITIY METHODS
    // ------

    // ! Might be deprecated, since now the parent-most object is the only one to possesses a Collider
    //   Recursively looks for the root prefab of a GameObject
    public GameObject FindPrefabOriginRecur(GameObject ARGprefabChild)
    {
        // Unfruitful search assumed from the start
        GameObject result = null;

        // Simplest case : the tested GameObject is the root prefab
        if (ARGprefabChild.tag == "InteractibleObject")
        {
            result = ARGprefabChild;
        }

        // Simplest case failed : going up the Parent ONLY IF the root of the scene was not reached
        else if (ARGprefabChild.gameObject != ARGprefabChild.GetComponentInParent<Transform>().gameObject)
            result = FindPrefabOriginRecur(ARGprefabChild.GetComponentInParent<Transform>().gameObject);

        return result;
    }
}
