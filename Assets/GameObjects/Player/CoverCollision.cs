using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CoverCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Murlock enemy = other.GetComponentInParent<Murlock>();
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
        }
        else if (other.gameObject.CompareTag("coverBreakable"))
        {
            print("Unknown object but apparently, we can remove it 👍");
            Destroy(other.gameObject);
        }
    }
}
