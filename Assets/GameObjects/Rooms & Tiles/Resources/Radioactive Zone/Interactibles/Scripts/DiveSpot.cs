using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveSpot : MonoBehaviour
{
    [NonSerialized] public List<GameObject> _linkedTiles = new();

    void Start()
    {
        Destroy(GetComponent<MeshRenderer>());

        Collider[] hits = Physics.OverlapBox(transform.position, new Vector3(2, 2, 2));
        foreach (Collider hit in hits)
        {
            if (hit.gameObject.CompareTag("TMTopology"))
            {
                _linkedTiles.Add(hit.gameObject);

                //// Looking for the highest tile if multiple ones are hit
                //foreach(GameObject target in _linkedTiles)
                //{
                //    if(hit.gameObject.transform.position.x == target.transform.position.x && hit.gameObject.transform.position.z == target.transform.position.z)
                //    {
                //        if((hit.gameObject.transform.position.y == target.transform.position.y)
                //    }
                //}
            }
        }
    }

}
