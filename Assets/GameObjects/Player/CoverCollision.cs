using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Spit spit) == false) return;
        if (spit.gameObject.CompareTag("coverBreakable") != true) return;

        print("Fuck you, "+other.gameObject.name);
        Destroy(other.gameObject);
        GI._PlayerFetcher().GetComponent<CoverManager>()._health -= 10;
    }
}
