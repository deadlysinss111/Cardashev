using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplicitInteractions : MonoBehaviour
{
    /*
     FIELDS 
    */
    GameObject _currentInteractible;
    PlayerManager _manager;
    Color _baseColor;

    /*
     PROPERTIES
    */
    // Nothing for now

    /*
     EVENTS
    */
    // Nothing for now

    /*
     METHODS
    */


    // Initialization
    private void Awake()
    {
        // Loading in PlayerManager a new state and its Action to change what the controls will do
        _manager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _manager.AddState("InteractibleTargeting", OnEnterState, OnExitState);
    }

    // Pair of Action for State changes
    private void OnEnterState()
    {
        // Sets up the functions for the controls
        _manager.SetLeftClickTo(RaycastResponseCaller);
        _manager.SetHoverTo(MouseOverPreview);
        _manager.SetRightClickTo( () => { } );

        // Extra force call to have the preview on the same frame the state changes
        MouseOverPreview();
    }
    private void OnExitState() { }


    // � Finds the root parent of the Interactible's prefab and highlight it
    // ! Changing hte materail color is a very bad idea, this need to change TODO
    public void MouseOverPreview()
    {
        // Raycast from the mouse, storing the hit if it succeeded
        RaycastHit raycastHit;
        if (false == Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit))
            return;

        // If it finds the GameObject's parent prefab, changes the GameManager's state and highlight the prefab
        GameObject raycastTarget = FindPrefabOriginRecur(raycastHit.transform.gameObject);
        if (raycastTarget != null)
        {
            // Restores the old material color if any Interactible was previously highlighted
            if (_currentInteractible != null)
                _currentInteractible.GetComponent<MeshRenderer>().material.color = _baseColor;

            // Updates the selected Interactible and saves its current material color
            _currentInteractible = raycastTarget;
            //_baseColor = _currentInteractible.GetComponent<MeshRenderer>().material.color;
            foreach(GameObject obj in _currentInteractible.GetComponentsInChildren<GameObject>())
            {
                _baseColor = obj.GetComponent<MeshRenderer>().material.color;
                obj.GetComponent<MeshRenderer>().material.color = new Color(0.5f, 0.0f, 0.5f, 1.0f);
            }
        }
    }

    // Calls the response to a raycast hit event of the Interactbile target
    public void RaycastResponseCaller()
    {
        _currentInteractible.GetComponent<Interactible>().OnRaycastHit();
    }


    // ------
    // UTILITIY METHODS
    // ------

    // Recursively looks for the root prefab of a GameObject
    public GameObject FindPrefabOriginRecur(GameObject ARGprefabChild)
    {
        // Unfruitful search assumed from the start
        GameObject result = null;

        // Simplest case : the tested GameObject is the root prefab
        if (ARGprefabChild.tag == "InteractibleObject")
            result = ARGprefabChild;

        // Simplest case failed : going up the Parent ONLY IF the root of the scene was not reached
        else if (ARGprefabChild.gameObject != ARGprefabChild.GetComponentInParent<Transform>().gameObject)
            result = FindPrefabOriginRecur(ARGprefabChild.GetComponentInParent<Transform>().gameObject);

        return result;
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }
}