using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineEffectScript : MonoBehaviour
{
    Animator _animator;

    [SerializeField] Image _top;
    [SerializeField] Image _bot;
    [SerializeField] Image _left;
    [SerializeField] Image _right;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void TakeDamageEffect()
    {
        _top.color = Color.red;
        _bot.color = Color.red;
        _left.color = Color.red;
        _right.color = Color.red;
        _animator.SetTrigger("TakeDamage");
    }

    public void SetTimeStopTo(bool value)
    {
        _top.color = Color.cyan;
        _bot.color = Color.cyan;
        _left.color = Color.cyan;
        _right.color = Color.cyan;
        _animator.SetBool("IsTimeStopped", value);
    }
}
