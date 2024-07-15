using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Acid : Interactible
{
    protected override void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<StatManager>().TakeDamage(10);
            return;
        }
        Mastodon mastodon;
        if (other.gameObject.TryGetComponent<Mastodon>(out mastodon))
        {
            mastodon.AcideBuff();
        }
            
    }
}
