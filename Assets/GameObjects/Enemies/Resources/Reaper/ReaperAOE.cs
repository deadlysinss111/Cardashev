using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaperAOE : MonoBehaviour
{
    [SerializeField] Reaper _self;
    BoxCollider _boxCollider;

    public List<Collider> _colliders;

    private void Start()
    {
        // If for some reason in the future, _self is not defined in the inspector, try to set its parent as _self as a fallback
        if (_self == null)
            _self = gameObject.GetComponentInParent<Reaper>();

        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.enabled = false;

        _self._changeOfStateScratch.AddListener(ReverseBox);
    }

    private void ReverseBox()
    {
        print((_boxCollider.enabled ? "Disable " : "Activate ") + "BoxCollider");
        _boxCollider.enabled = !_boxCollider.enabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        _colliders.Add(other);
        if (other.gameObject.CompareTag("Player") && other.gameObject.TryGetComponent(out StatManager _stats))
        {
            _stats.TakeDamage(_self._dmg);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _colliders.Remove(other);
    }
}
