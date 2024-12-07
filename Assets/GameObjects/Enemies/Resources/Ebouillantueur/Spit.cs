using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spit : MonoBehaviour
{
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
                if(c.gameObject.GetComponent<Mastodon>() != null)
                {
                    c.gameObject.GetComponent<Mastodon>().AcideBuff();
                }

                // Deals damage if it lands on anything else than fish and bouilloir
                else if(c.gameObject.GetComponent<Ebouillantueur>() == null)
                {
                    manager.TakeDamage(20);
                }
            }

            // Summons an Interactible if it hits the ground
            else if(c.gameObject.CompareTag("TMTopology"))
            {
                RaycastHit hit;
                if(Physics.Raycast(transform.position , Vector3.down, out hit))
                {
                    Instantiate(Resources.Load("Radioactive Zone/Interactibles/Prefabs/Acid"), hit.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                    //print(hit.transform.gameObject.name);
                    break;
                }
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
                if (c.gameObject.GetComponent<Mastodon>() != null)
                {
                    c.gameObject.GetComponent<Mastodon>().AcideBuff();
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
