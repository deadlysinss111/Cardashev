using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        StatManager manager;
        // We make sure to go down, otherwise we don't explode on contact
        //if (GetComponent<Rigidbody>().velocity.y > 0)
        //    return;

        // Grenade explosion on ground hit
        Collider[] hits = Physics.OverlapSphere(transform.position, 5);
        foreach (Collider c in hits)
        {
            if (c.gameObject.TryGetComponent<StatManager>(out manager))
            {
                manager.TakeDamage(10);
                //break;
            }
            GameObject target = HierarchySearcher.FindParentdRecursively(c.transform, "Body");
            if (target != null)
            {
                target.transform.parent.gameObject.GetComponent<StatManager>().TakeDamage(10);
            }
        }
        Destroy(gameObject);
    }
}
