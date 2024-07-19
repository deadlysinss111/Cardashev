using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeleteMe : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            print("dpgi,");
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit rcHit);
            if (NavMesh.SamplePosition(rcHit.point, out NavMeshHit hit, 100, -1))
                GetComponent<NavMeshAgent>().SetDestination(hit.position);
            else print("ratio");
        }
    }
}
