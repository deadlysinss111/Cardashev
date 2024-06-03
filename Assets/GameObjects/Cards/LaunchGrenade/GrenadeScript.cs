using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        StatManager manager;
        if (GetComponent<Rigidbody>().velocity.y > 0)
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, 5);
        foreach (Collider c in hits)
        {
            if (c.gameObject.TryGetComponent<StatManager>(out manager))
            {
                manager._health -= 10;
            }
        }
        Destroy(gameObject);
    }
}
