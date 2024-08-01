using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QueueUI : MonoBehaviour
{
    public GameObject _segmentPrefab; // Reference to the segment prefab

    QueueComponent _queue;
    Image _secsBar;
    TMP_Text _secsText;
    float _barScale;
    Queue<GameObject> _segmentBars;
    bool _dequeuAsk;
    Transform _bg;

    void Start()
    {
        _segmentBars = new Queue<GameObject>();
        _queue = GI._PlayerFetcher().GetComponent<QueueComponent>();
        _bg = transform.Find("Background");
        _secsBar = _bg.GetComponent<Image>();
        _barScale = _secsBar.transform.localScale.x;
        _secsText = transform.Find("Seconds").GetComponent<TMP_Text>();
    }

    void Update()
    {
        _secsText.text = Math.Round(_queue.TotalQueueTime(), 1) + "s";
        DisplayQueue();
    }


    void DisplayQueue()
    {

        float offset = 0;
        foreach(GameObject segment in _segmentBars.ToList())
        {
            UpdateSegment(segment, ref offset);
        }
        if (_dequeuAsk)
        {
            Destroy(_segmentBars.Dequeue());
            _dequeuAsk = false;
        }
    }

    void UpdateSegment(GameObject segment, ref float offset)
    {
        Vector3 scale = segment.transform.localScale;
        if (segment == _segmentBars.Peek())
        {
            if (_queue.IsCurrentCardEmpty())
            {
                _dequeuAsk = true;
                return;
            }

            float cardTime = _queue.GetActiveCard().GetRemainingTime();
            if (cardTime <= 0)
            {
                _segmentBars.Dequeue();
                return;
            }

            segment.transform.localScale = DurationToScale(cardTime);
        }

        Vector3 pos = segment.transform.localPosition;
        // Look at here if the original position of segments have an issue (the -800 is half the size of the queubar, maybe it has been rescaled or smth)
        pos.x = -800 + offset;
        segment.transform.localPosition = pos;
        offset += segment.GetComponent<RectTransform>().rect.width * segment.transform.localScale.x;
    }

    private Vector3 DurationToScale(float duration)
    {
        // Sets the scale of the bar based on the queue's total time
        Vector3 scale = _secsBar.transform.localScale;
        scale.x = (duration / _queue._MaxTimeBuffer) * _barScale;
        return scale;
    }

    public void AddSegment(float duration, Color color)
    {
        GameObject segment = Instantiate(_segmentPrefab, _secsBar.transform);
        print("shader is : " + Resources.Load<Material>("ShadeBordersMaterial"));
        segment.GetComponent<Image>().material = Instantiate(Resources.Load<Material>("ShadeBordersMaterial"));
        segment.GetComponent<Image>().material.color = color;
        segment.transform.localScale = DurationToScale(duration);
        _segmentBars.Enqueue(segment);
    }
}
