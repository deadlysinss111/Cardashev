using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobAndWincon : MonoBehaviour
{
    [SerializeField] List<string> _winConditions;

    // May be usefull later on
    GameObject _exitTile;

    void Start()
    {
        _exitTile = GameObject.Find("ExitTile(Clone)");
        EscapeTile esc = HierarchySearcher.FindChildRecursively(_exitTile.transform, "ExitPlate").GetComponent<EscapeTile>();

        foreach(string name in _winConditions)
        {
            esc.AddCondition(name);
        }
    }
}
