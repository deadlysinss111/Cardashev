using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spit : MonoBehaviour
{
    public bool _shieldBrekable = true;

    private void OnCollisionEnter(Collision other)
    {
        // TODO: this is a big band-aid, we'd like to find a better way to do that
        StatManager manager;
        if (GetComponent<Rigidbody>().velocity.y > 0)
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, 2);
        foreach (Collider c in hits)
        {
            if (c.gameObject.TryGetComponent<StatManager>(out manager))
            {
                // Buffs a fish if the projectile lands on it
                if(c.gameObject.GetComponent<Murlock>() != null)
                {
                    c.gameObject.GetComponent<Murlock>().AcideBuff();
                }

                // Deals damage if it lands on anything else than fish and bouilloir
                else if(c.gameObject.GetComponent<Ebouillantueur>() == null)
                {
                    manager.TakeDamage(10);
                }
            }

            // Summons an Interactible if it hits the ground
            else if(HierarchySearcher.FindParentdRecursively(c.gameObject.transform, "Topology") != null)
            {
                RaycastHit hit;
                Physics.Raycast(transform.position, Vector3.down, out hit);
                Instantiate(Resources.Load("Debug Zone/Interactibles/Prefabs/Acid"), hit.transform.position + new Vector3(0, 0.6f, 0), Quaternion.identity);
                break;
            }
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: this is a big band-aid, we'd like to find a better way to do that
        StatManager manager;

        Collider[] hits = Physics.OverlapSphere(transform.position, 2);
        foreach (Collider c in hits)
        {
            if (c.gameObject.TryGetComponent<StatManager>(out manager))
            {
                // Buffs a fish if the projectile lands on it
                if (c.gameObject.GetComponent<Murlock>() != null)
                {
                    c.gameObject.GetComponent<Murlock>().AcideBuff();
                }

                // Deals damage if it lands on anything else than fish and bouilloir
                else if (c.gameObject.GetComponent<Ebouillantueur>() == null)
                {
                    manager.TakeDamage(10);
                }
            }
        }
        Destroy(gameObject);
    }
}
