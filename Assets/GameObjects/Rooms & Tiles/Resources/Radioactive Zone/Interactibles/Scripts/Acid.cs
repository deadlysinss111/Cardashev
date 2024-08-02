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
            GI._PStatFetcher().AddModifier(new StatManager.Modifier(StatManager.Modifier.ModifierType.RadPoison, 2.0f, 5.0f, 1.0f));
            return;
        }
        Mastodon mastodon;
        if (other.gameObject.TryGetComponent<Mastodon>(out mastodon))
        {
            mastodon.AcideBuff();
        }
            
    }
}
