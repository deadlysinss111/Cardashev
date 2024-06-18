using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HierarchySearcher
{
    // Genric methods to get a parent recusively by name. Gift of ChatGPT
    static public GameObject FindParentdRecursively(Transform target, string ARGchildName)
    {
        if(target.name == ARGchildName)
            return target.gameObject;
        return INTERNALFindParentRec(target, ARGchildName);
    }

    static private GameObject INTERNALFindParentRec(Transform child, string ARGchildName)
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

    // Generic methods to get a child recusively by name. Gift of ChatGPT
    static public GameObject FindChildRecursively(Transform target, string ARGchildName)
    {
        if (target.name == ARGchildName)
            return target.gameObject;
        return INTERNALFindChildRec(target.transform, ARGchildName);
    }

    static private GameObject INTERNALFindChildRec(Transform parent, string ARGchildName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == ARGchildName)
                return child.gameObject;

            GameObject result = INTERNALFindChildRec(child, ARGchildName);
            if (result != null)
                return result;
        }
        return null;
    }

    // Generic method to get a child by name
    static public GameObject FindChild(Transform target, string ARGchildName)
    {
        foreach (Transform child in target.transform)
            if (child.name == ARGchildName)
                return child.gameObject;
        return null;
    }
}
