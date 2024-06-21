using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Fuck you, "+other.gameObject.name);
        Destroy(other.gameObject);
        GI._PlayerFetcher().GetComponent<CoverManager>()._health -= 10;
    }
}
