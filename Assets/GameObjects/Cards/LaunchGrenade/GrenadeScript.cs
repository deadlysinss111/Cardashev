using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public Vector3 _velocity;
    public Vector3 _origin;
    public int _dmg;

    private void Start()
    {
        StartCoroutine(LaunchTheGrenadeFromHand());
        GetComponent<Rigidbody>().isKinematic = true;
    }


    private void OnCollisionEnter(Collision other)
    {
        StatManager manager;
        // We make sure to go down, otherwise we don't explode on contact
        //if (GetComponent<Rigidbody>().velocity.y > 0)
        //    return;

        // Grenade explosion on ground hit
        Collider[] hits = Physics.OverlapSphere(transform.position, 3);
        foreach (Collider c in hits)
        {
            if (c.gameObject.TryGetComponent<StatManager>(out manager))
            {
                manager.TakeDamage(_dmg);
                //break;
            }
            GameObject target = HierarchySearcher.FindParentdRecursively(c.transform, "Body");
            if (target != null)
            {
                // Alterate the target in case we hit an object structured with an Animator (Jackhammer only atm)
                if (HierarchySearcher.FindParentdRecursively(target.transform, "Animator") != null)
                {
                    print("altered");
                    target = HierarchySearcher.FindParentdRecursively(target.transform, "Animator");
                }
                if(target.transform.parent.gameObject.TryGetComponent<StatManager>(out StatManager statManager))
                    statManager.TakeDamage(_dmg);
            }
        }
        Destroy(gameObject);
    }

    IEnumerator LaunchTheGrenadeFromHand()
    {
        while (GI._PlayerFetcher().GetComponent<QueueComponent>().GetActiveCard().GetRemainingTime() > 0.5)
        {
            yield return null;
        }

        GetComponent<Rigidbody>().isKinematic = false;
        transform.position = _origin;
        GetComponent<Rigidbody>().velocity = _velocity;
    }
}
