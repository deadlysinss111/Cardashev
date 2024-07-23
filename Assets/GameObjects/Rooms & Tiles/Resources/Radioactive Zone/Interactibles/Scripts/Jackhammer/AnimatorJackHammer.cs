using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class AnimatorJackHammer : MonoBehaviour
{
    public void Move()
    {
        HierarchySearcher.FindParentdRecursively(transform, "Jackhammer").GetComponent<JackHammer>().Move();
    }
    public void Bake()
    {
        GameObject.Find("RoomAnchor").GetComponent<NavMeshSurface>().BuildNavMesh();
    }

}
