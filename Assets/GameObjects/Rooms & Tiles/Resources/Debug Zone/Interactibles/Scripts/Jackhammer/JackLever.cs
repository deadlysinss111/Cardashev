using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class JackLever : Interactible
{
    bool _startuping;
    [SerializeField] int _explosionDmg = 40;

    private void Start()
    {
        _RaycastHitDist = 15;
        _startuping = false;
    }

    public override void OnRaycastHit()
    {
        if (Time.timeScale == 0 || _startuping) return;

        // On object interaction, we add to queue the activation of the hammer
        if (Vector3.Distance(this.transform.position, GI._PManFetcher()._virtualPos) <= _RaycastHitDist)
        {
            GameObject moveCardObj = new GameObject();
            Card moveCard = moveCardObj.AddComponent<Card>();
            moveCard._duration = 2;
            moveCard._trigger += () =>
            {
                StartCoroutine(ActivationCoroutine(moveCard._duration));
            };
            _playerRef.GetComponent<QueueComponent>().AddToQueue(moveCard);
            _startuping = true;
            if (TryGetComponent<Outline>(out Outline outline))
                Destroy(outline);
            _isHiglightable = false;
        }
        else
            print("you're too far from the hammer");
    }

    private IEnumerator ActivationCoroutine(float duration)
    {
        while(duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        HierarchySearcher.FindParentdRecursively(transform, "Jackhammer(Clone)").GetComponent<JackHammer>().Operating(true);
    }

    public override void Kill()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 3);
        foreach (Collider c in hits)
        {
            if (c.gameObject == HierarchySearcher.FindChildRecursively(transform, "Body") || c.gameObject == gameObject) continue;

            {
                
            }
            if (c.gameObject.TryGetComponent<StatManager>(out StatManager manager))
            {
                manager.TakeDamage(_explosionDmg);
                //break;
            }
            GameObject target = HierarchySearcher.FindParentdRecursively(c.transform, "Body");
            if (target != null)
            {
                // Alterate the target in case we hit an object structured with an Animator (Jackhammer only atm)
                if (HierarchySearcher.FindParentdRecursively(target.transform, "Animator") != null)
                {
                    print("altered");
                    target = HierarchySearcher.FindParentdRecursively(target.transform, "Animator");
                }
                if (target.transform.parent.gameObject.TryGetComponent<StatManager>(out StatManager statManager))
                    statManager.TakeDamage(_explosionDmg);
            }
        }
        base.Kill();
    }
}
