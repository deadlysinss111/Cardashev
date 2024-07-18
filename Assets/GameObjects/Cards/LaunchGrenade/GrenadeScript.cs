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
                DoDamage(c.gameObject);
                continue;
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
                DoDamage(target.transform.parent.gameObject);
            }
        }
        Instantiate((GameObject)Resources.Load("GrenadeAOE")).transform.position = transform.position;
        Destroy(gameObject);
    }

    void DoDamage(GameObject target)
    {
        if(target.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(_dmg);
        }
        else if(target.TryGetComponent(out StatManager manager))
        {
            manager.TakeDamage(_dmg);
        }
        else
        {
            print("error in dealing dmg with target : "+target.name);
        }
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
