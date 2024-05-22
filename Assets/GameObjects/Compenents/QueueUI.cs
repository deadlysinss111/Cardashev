using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QueueUI : MonoBehaviour
{
    // TODO: Get that from the player instead
    private QueueComponent queue;

    Image secs_bar;
    TMP_Text secs_text;
    float bar_scale;
    float bar_max = 10f;

    // Start is called before the first frame update
    void Start()
    {
        queue = GameObject.Find("Queue").GetComponent<QueueComponent>();

        secs_bar = transform.Find("Foreground").GetComponent<Image>();
        bar_scale = secs_bar.transform.localScale.x;
        secs_text = transform.Find("Seconds").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Sets the scale of the bar based on the queue's total time
        Vector3 scale = secs_bar.transform.localScale;
        scale.x = (queue.TotalQueueTime() / bar_max) * bar_scale;
        secs_bar.transform.localScale = scale;
        // Rounds the time and sets the text
        secs_text.text = Math.Round(queue.TotalQueueTime(), 1) + "s";
    }
}
