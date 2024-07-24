using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomIconAnim : MonoBehaviour
{
    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Select()
    {
        _animator.SetTrigger("Select");
    }

    public void MouseEnter()
    {
        _animator.SetTrigger("MouseEnter");
        if (!_animator.GetBool("MouseHover"))
            _animator.SetBool("MouseHover", true);
    }

    public void MouseExit()
    {
        _animator.SetBool("MouseHover", false);
    }

    public void Lock()
    {
        _animator.SetTrigger("Lock");
    }
}
