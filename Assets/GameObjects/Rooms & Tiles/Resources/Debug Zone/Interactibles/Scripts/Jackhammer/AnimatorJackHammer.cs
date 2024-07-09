using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorJackHammer : MonoBehaviour
{
    public void Move()
    {
        HierarchySearcher.FindParentdRecursively(transform, "Jackhammer(Clone)").GetComponent<JackHammer>().Move();
    }
}
