using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

/*
 • Interactible's base abstract class, containing virtual effects TBD in actual classes, but every functionnality to trigger Interactbiles
*/
public abstract class Interactible : MonoBehaviour
{
    /*
     FIELDS 
    */
    // Yes, there is one ! This is useful to quickly determine the distance of the Interactible to the player
    protected static GameObject _playerRef;

    /* 
     PROPERTIES
    */
    // Properties that don't need to be implemented differently in child classes
    // RaycastHit is useless outisde of this class, but the destructible flag isn't (so its `get` is public)
    protected float _RaycastHitDist { get; set; }
    public bool _IsDestructible { get; protected set; }
    // Any extra properties are in their respective sub-classes


    /* 
     EVENTS
    */
    public UnityEvent _UeOnRaycastHit;


    /* 
     METHODS
    */
    protected void OnTriggerEnter(Collider ARGcollider)
    {
        switch (ARGcollider.tag)
        {
            case "Player":
                Debug.Log("Player came into contact with " + this.gameObject.name);
                break;

            case "Enemy":
                Debug.Log(ARGcollider.gameObject.name + "Enemy came into contact with " + this.gameObject.name);
                break;

            // Should be repalces by more case, since this is intended to detect projectiles, attack hitboxes or explosions
            // TODO : add more case when those 3 will be implemeneted
            default:
                Debug.Log("Something else came into contact with " + this.gameObject.name);
                break;
        }
    }

    protected void OnTriggerExit(Collider ARGcollider)
    {
        switch (ARGcollider.tag)
        {
            case "Player":
                Debug.Log("Player came ended with " + this.gameObject.name);
                break;

            case "Enemy":
                Debug.Log(ARGcollider.gameObject.name + "Enemy came ended with " + this.gameObject.name);
                break;

            // Should be repalced by more case, since this is intended to detect projectiles, attack hitboxes or explosions
            // TODO : add more case when those 3 will be implemeneted
            default:
                Debug.Log("Something else ended contact with " + this.gameObject.name);
                break;
        }
    }

    protected void OnTriggerStay(Collider ARGcollider)
    {
        // Pause time :3
    }

    public void OnRaycastHit()
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
        Debug.Log("auuuuUUUUUuugh");
        // Changes the PlayerManager state
        _playerRef.GetComponent<PlayerManager>().SetToState("InteractibleTargeting");
    }
    private void OnMouseExit()
    {
        // Restores the previous state
        _playerRef.GetComponent<PlayerManager>().SetToDefult();
    }


    protected void Awake()
    {
        // Field setup
        _playerRef = GameObject.Find("Player").gameObject;

        // Event subscribing
        _UeOnRaycastHit.AddListener(OnRaycastHit);

        // Mesh Combining
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        List<CombineInstance> combine = new List<CombineInstance>();

        foreach (Transform child in this.transform)
            if (child.GetComponent<MeshFilter>() != null)
                meshFilters.Add(child.GetComponent<MeshFilter>());

        for (int i = 0;  i < meshFilters.Count ; ++i)
        {
            // Stored current meshFilter data since we need to read it a bunch
            MeshFilter curMeshFilter = meshFilters[i];
            Transform curMFTransfrom = curMeshFilter.transform;

            // Filling out a temp combineInstance and adding it to the combine List
            CombineInstance combInstTemp = new CombineInstance();
            combInstTemp.mesh = curMeshFilter.sharedMesh;
            combInstTemp.transform = Matrix4x4.TRS(curMFTransfrom.localPosition, curMFTransfrom.localRotation, curMFTransfrom.localScale);
            Debug.Log("transform : " + meshFilters[i].transform.localPosition);
            combine.Add(combInstTemp);
        }

        // Combining meshes together, and activating it
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine.ToArray());
        transform.GetComponent<MeshFilter>().sharedMesh = combinedMesh;
        transform.GetComponent<MeshCollider>().sharedMesh = combinedMesh;
        gameObject.SetActive(true);
    }
}
