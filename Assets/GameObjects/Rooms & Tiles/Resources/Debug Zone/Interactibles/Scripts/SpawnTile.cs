using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnTile : Interactible
{
    private void Start()
    {
        GameObject player = GI._PlayerFetcher();
        player.GetComponent<NavMeshAgent>().enabled = false;
        player.transform.position = transform.position + new Vector3(0, 1, 0);
        GI._PManFetcher()._virtualPos = player.transform.position;
        player.transform.rotation = transform.rotation;

        player.GetComponent<NavMeshAgent>().enabled = true;
        StartCoroutine(ThinkingTime());
    }

    IEnumerator ThinkingTime()
    {
        QueueComponent queue = GI._PlayerFetcher().GetComponent<QueueComponent>();
        Time.timeScale = 0;
        while(queue.GetQueue().Count == 0 && queue.IsCurrentCardEmpty())
        {
            yield return null;
        }
        Time.timeScale = 1;
    }
}