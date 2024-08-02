using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    Vector3 _size;
    Material _material;
    GameObject _hologram;

    private void Start()
    {
        _hologram = HierarchySearcher.FindChildRecursively(transform, "HologramHexagon");
        _size = transform.localScale;
        _material = Instantiate(Resources.Load<Material>("ButtonHologramMaterial"));
        _hologram.GetComponent<MeshRenderer>().material = _material;
    }

    private void OnMouseEnter()
    {
        transform.localScale = transform.localScale * 1.2f;
        GetComponent<Animator>().SetBool("MouseOn", true);
    }

    protected void OnMouseExit()
    {
        transform.localScale = _size;
        GetComponent<Animator>().SetBool("MouseOn", false);
    }
}