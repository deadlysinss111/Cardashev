using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QueueUI : MonoBehaviour
{
    private QueueComponent _queue;

    Image _secsBar;
    TMP_Text _secsText;
    float _barScale;

    // Start is called before the first frame update
    void Start()
    {
        _queue = GameObject.Find("Player").GetComponent<QueueComponent>();

        _secsBar = transform.Find("Foreground").GetComponent<Image>();
        _barScale = _secsBar.transform.localScale.x;
        _secsText = transform.Find("Seconds").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Sets the scale of the bar based on the queue's total time
        Vector3 scale = _secsBar.transform.localScale;
        scale.x = (_queue.TotalQueueTime() / _queue._MaxTimeBuffer) * _barScale;
        _secsBar.transform.localScale = scale;
        // Rounds the time and sets the text
        _secsText.text = Math.Round(_queue.TotalQueueTime(), 1) + "s";
    }
}
