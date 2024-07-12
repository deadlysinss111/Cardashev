using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CoverCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Murlock enemy = other.gameObject.GetComponentInParent<Murlock>();
        if (other.gameObject.TryGetComponent(out Spit spit))
        {
            print("Fuck you, " + other.gameObject.name);
            Destroy(other.gameObject);

            //GI._PManFetcher().GetComponent<StatManager>().TakeDamage();
            GI._PlayerFetcher().GetComponent<CoverManager>()._health -= 10;
        }
        else if (enemy != null)
        {
            print("Begone, fish!");

            enemy.InterruptAct();
            if (enemy.TryGetComponent(out Rigidbody rBody))
            {
                print("Push back!");
                rBody.velocity = Vector3.Reflect(other.relativeVelocity, other.contacts[0].normal); ;
            }
        }
        else if (other.gameObject.CompareTag("coverBreakable"))
        {
            print("Unknown object but apparently, we can remove it 👍");
            Destroy(other.gameObject);
        }
    }
}
