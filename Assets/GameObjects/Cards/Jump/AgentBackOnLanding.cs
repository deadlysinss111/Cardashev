using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentBackOnLanding : MonoBehaviour
{
    private void OnTriggerEnter(Collider target)
    {
        if(FindParentdRecursively(target.gameObject.transform, "Topology") != null && gameObject.GetComponent<Rigidbody>().velocity.y <=0)
        {
            gameObject.GetComponent<NavMeshAgent>().enabled = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            Destroy(this);
        }
    }

    public GameObject FindParentdRecursively(Transform target, string ARGchildName)
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
