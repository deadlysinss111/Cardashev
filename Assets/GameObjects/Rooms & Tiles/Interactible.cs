using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

// Interactible's base abstract class, containing virtual effects TBD in actual classes, but every functionnality to trigger Interactbiles
public abstract class Interactible : MonoBehaviour
{
    /*
     FIELDS 
    */
    protected static GameObject _playerRef;
    protected static PlayerManager _playerManager;
    [SerializeField] bool _isHiglightable = true;
    bool _selectable = false;

    /* 
     PROPERTIES
    */
    // Properties that don't need to be implemented differently in child classes
    // RaycastHit is useless outisde of this class, but the destructible flag isn't (so its `get` is public)
    protected float _RaycastHitDist { get; set; }
    public bool _IsDestructible { get; protected set; }
    public bool IsSelectable { get { return _selectable; } }
    // Any extra properties are in their respective sub-classes


    /* 
     EVENTS
    */
    public UnityEvent _UeOnRaycastHit;


    /* 
     METHODS
    */
    // ------
    // INITIALIZATION
    // ------

    protected void Awake()
    {
        // Field setup
        _playerRef = GameObject.Find("Player").gameObject;
        _playerManager = _playerRef.GetComponent<PlayerManager>();
        _RaycastHitDist = 10.0f;

        // Event subscribing
        _UeOnRaycastHit.AddListener(OnRaycastHit);

        // Non combined mesh scenario
        if (transform.childCount == 0) return;

        // Mesh Combining
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        List<CombineInstance> combine = new List<CombineInstance>();

        foreach (Transform child in this.transform)
            if (child.GetComponent<MeshFilter>() != null)
                meshFilters.Add(child.GetComponent<MeshFilter>());

        for (int i = 0; i < meshFilters.Count; ++i)
        {
            // Stored current meshFilter data since we need to read it a bunch
            MeshFilter curMeshFilter = meshFilters[i];
            Transform curMFTransfrom = curMeshFilter.transform;

            // Filling out a temp combineInstance and adding it to the combine List
            CombineInstance combInstTemp = new CombineInstance();
            combInstTemp.mesh = curMeshFilter.sharedMesh;
            combInstTemp.transform = Matrix4x4.TRS(curMFTransfrom.localPosition, curMFTransfrom.localRotation, curMFTransfrom.localScale);
            combine.Add(combInstTemp);
        }

        // Combining meshes together, and activating it
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine.ToArray());
        transform.GetComponent<MeshFilter>().sharedMesh = combinedMesh;
        transform.GetComponent<MeshCollider>().sharedMesh = combinedMesh;
        gameObject.SetActive(true);
    }


    // ------
    // INTERACTIONS CALLBACKS
    // ------

    virtual protected void OnTriggerEnter(Collider ARGcollider)
    {
        switch (ARGcollider.gameObject.tag)
        {
            case "Player":
                Debug.Log("Player came into contact with " + this.gameObject.name);
                break;

            case "Enemy":
                Debug.Log(ARGcollider.gameObject.name + "Enemy came into contact with " + this.gameObject.name);
                break;

            // Should be replaced by more case, since this is intended to detect projectiles, attack hitboxes or explosions
            // TODO : add more case when those 3 will be implemented
            default:
                Debug.Log("Something else came into contact with " + this.gameObject.name);
                break;
        }
    }

    virtual protected void OnTriggerExit(Collider ARGcollider)
    {
        switch (ARGcollider.gameObject.tag)
        {
            case "Player":
                Debug.Log("Player left contact with " + this.gameObject.name);
                break;

            case "Enemy":
                Debug.Log(ARGcollider.gameObject.name + "Enemy left contact with " + this.gameObject.name);
                break;

            // Should be replaced by more case, since this is intended to detect projectiles, attack hitboxes or explosions
            // TODO : add more case when those 3 will be implemented
            default:
                Debug.Log("Something else left contact with " + this.gameObject.name);
                break;
        }
    }

    virtual protected void OnTriggerStay(Collider ARGcollider) { }

    virtual public void OnRaycastHit()
    {
        // Tests the distance of the player from the one of the Interactible in all 3 axis
        if (Vector3.Distance(this.transform.position, _playerRef.transform.position) <= _RaycastHitDist)
            Debug.Log(this.gameObject.name + " was raycast-hit within valid distance !");
        else
            Debug.Log(this.gameObject.name + " was raycast-hit within invalid distance !");
    }


    // ------
    // STATE WATCHER
    // ------

    private void OnMouseEnter()
    {
        if (_isHiglightable)
        {
            // Adds the Outline component
            Outline outlineComp = this.AddComponent<Outline>();
            outlineComp.OutlineMode = Outline.Mode.OutlineAll;
            outlineComp.OutlineColor = Color.green;
            outlineComp.OutlineWidth = 4.7f;

            // Changes the PlayerManager state and tells it it should do a MouseHover check since what's under the mouse just changed
            _playerManager.SetToState("InteractibleTargeting");
            _playerManager.TriggerMouseHovering();
        }
    }
    private void OnMouseExit()
    {
        if (_isHiglightable)
        {
            // Removes the Outline component
            Outline outlineComp = this.GetComponent<Outline>();
            if (outlineComp != null)
                Destroy(outlineComp);

            // Restores the previous state
            _playerManager.SetToLastState();
        }
    }


    

    void Update()
    {
        CheckSelectable();
    }

    void CheckSelectable()
    {
        _selectable = false;
        //transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.white;

        if (SelectableArea.InteractableAreaCheck == false) return;

        int layerMask = (1 << LayerMask.NameToLayer("Player"));
        layerMask |= (1 << LayerMask.NameToLayer("Enemy"));
        layerMask = ~layerMask;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10, layerMask) == false) return;

        GameObject hitObj = hit.transform.gameObject;
        if (hitObj.TryGetComponent(out Tile tile) == false) return;

        if (tile.IsSelectable == false) return;

        SetSelected(true);
    }

    public void SetSelected(bool value)
    {
        print($"Interactable {gameObject.name} is selectable!");
        _selectable = value;

        Outline outline;
        if (_selectable && TryGetComponent<Outline>(out _) == false)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = Color.red;
            outline.OutlineWidth = 7.7f;
        }
        else if (_selectable == false && TryGetComponent<Outline>(out outline))
        {
            Destroy(outline);
        }
    }
}
