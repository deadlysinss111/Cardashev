using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    Vector3 _size;

    private void Start()
    {
        _size = transform.localScale;
    }

    private void OnMouseEnter()
    {
        transform.localScale = transform.localScale * 1.2f;
    }

    protected void OnMouseExit()
    {
        transform.localScale = _size;
    }
}
