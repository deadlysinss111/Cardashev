using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spit : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        StatManager manager;
        if (GetComponent<Rigidbody>().velocity.y > 0)
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, 2);
        foreach (Collider c in hits)
        {
            if (c.gameObject.TryGetComponent<StatManager>(out manager))
            {
                if(c.gameObject.GetComponent<Murlock>() != null)
                {
                    c.gameObject.GetComponent<Murlock>().AcideBuff();
                }
                else if(c.gameObject.GetComponent<Ebouillantueur>() == null)
                {
                    manager._health -= 10;
                }
            }
            else if(FindParentdRecursively(c.gameObject.transform, "Topology") != null)
            {
                RaycastHit hit;
                Physics.Raycast(transform.position, Vector3.down, out hit);
                Instantiate(Resources.Load("Debug Zone/Interactibles/Prefabs/Acid"), hit.transform.position + new Vector3(0, 0.6f, 0), Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }

    // Genric methods to get a parent recusively by name. Gift of ChatGPT
    public GameObject FindParentdRecursively(Transform target,string ARGchildName)
    {
        return INTERNALFindParentRec(target, ARGchildName);
    }

    private GameObject INTERNALFindParentRec(Transform child, string ARGchildName)
    {
        Transform parent = child.parent;
        if (parent != null)
        {
            if (parent.name == ARGchildName)
                return parent.gameObject;

            GameObject result = INTERNALFindParentRec(parent, ARGchildName);
            if (result != null)
                return result;
        }
        return null;
    }
}