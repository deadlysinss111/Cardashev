using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Android.Types;
using UnityEngine;
using UnityEngine.VFX;

public class GrenadeScript : MonoBehaviour
{
    public Vector3 _velocity;
    public Vector3 _origin;
    public int _dmg;
    public int _explosionRadius;
    public GameObject _explosionEffect;

    private void Start()
    {
        StartCoroutine(LaunchTheGrenadeFromHand());
        GetComponent<Rigidbody>().isKinematic = true;
    }


    private void OnCollisionEnter(Collision other)
    {
        // We make sure to go down, otherwise we don't explode on contact
        //if (GetComponent<Rigidbody>().velocity.y > 0)
        //    return;

        // Grenade explosion on ground hit
        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius/2);
        foreach (Collider c in hits)
        {
            if (c.gameObject.TryGetComponent(out StatManager _))
            {
                DoDamage(c.gameObject);
                continue;
            }
            GameObject target = HierarchySearcher.FindParentdRecursivelyWithScript(c.transform, (Transform target) => { return target.gameObject.TryGetComponent<Enemy>(out _); });
            if (target != null)
            {
                DoDamage(target);
                continue;
            }
            target = HierarchySearcher.FindParentdRecursivelyWithScript(c.transform, (Transform target) => { return target.gameObject.TryGetComponent<Interactible>(out _); });
            if (target != null)
            {
                DoDamage(target);
                continue;
            }
        }
        //GameObject temp = Instantiate((GameObject)Resources.Load("GrenadeAOE"));
        //temp.transform.position = transform.position;
        //temp.transform.localScale = new Vector3(_explosionRadius, _explosionRadius, _explosionRadius);
        GameObject e = GameObject.Instantiate(_explosionEffect, transform.position, transform.rotation);
        //e.transform.localScale = Vector3.one * (_explosionRadius * 2);
        foreach (Transform item in e.transform)
        {
            item.gameObject.transform.localScale = Vector3.one * _explosionRadius/4;
        }
        Destroy(gameObject);
    }

    void DoDamage(GameObject target)
    {
        print(target.name);
        if(target.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(_dmg);
        }
        else if(target.TryGetComponent(out StatManager manager))
        {
            manager.TakeDamage(_dmg);
        }
        else
        {
            print("error in dealing dmg with target : "+target.name);
        }
    }

    IEnumerator LaunchTheGrenadeFromHand()
    {
        while (GI._PlayerFetcher().GetComponent<QueueComponent>().GetActiveCard().GetRemainingTime() > 0.5)
        {
            yield return null;
        }

        GetComponent<Rigidbody>().isKinematic = false;
        transform.position = _origin;
        GetComponent<Rigidbody>().velocity = _velocity;
    }
}
