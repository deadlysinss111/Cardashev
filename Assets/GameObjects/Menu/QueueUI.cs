using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QueueUI : MonoBehaviour
{
    public GameObject segmentPrefab; // Reference to the segment prefab

    QueueComponent _queue;
    Image _secsBar;
    TMP_Text _secsText;
    float _barScale;
    List<GameObject> _segmentBars = new List<GameObject>();
    GameObject _activeSegment;

    void Start()
    {
        _queue = GI._PlayerFetcher().GetComponent<QueueComponent>();
        _secsBar = transform.Find("Background").GetComponent<Image>();
        _barScale = _secsBar.transform.localScale.x;
        _secsText = transform.Find("Seconds").GetComponent<TMP_Text>();
    }

    void Update()
    {
        _secsText.text = Math.Round(_queue.TotalQueueTime(), 1) + "s";
        DisplaySegmentedTimers();
        UpdateActiveSegment();
    }

    void DisplaySegmentedTimers()
    {
        // Clear previous segments
        foreach (GameObject segment in _segmentBars)
        {
            Destroy(segment);
        }
        _segmentBars.Clear();

        float cumulativeTime = 0;
        bool activeSegmentSet = false;

        foreach (var action in _queue.GetQueue())
        {
            float segmentWidth = (action._duration / _queue._MaxTimeBuffer) * _barScale;

            GameObject segmentObj = Instantiate(segmentPrefab, _secsBar.transform);
            Image segmentImage = segmentObj.GetComponent<Image>();
            segmentImage.color = action._actionColor;
            _segmentBars.Add(segmentObj);
            Debug.Log("Segment width: " + segmentWidth);

            RectTransform rectTransform = segmentObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(cumulativeTime / _queue._MaxTimeBuffer, 0.5f);
            rectTransform.anchorMax = new Vector2((cumulativeTime + action._duration) / _queue._MaxTimeBuffer, 0.5f);
            rectTransform.pivot = new Vector2(0, 0.5f);
            rectTransform.offsetMin = new Vector2(0, -_secsBar.rectTransform.rect.height / 2);
            rectTransform.offsetMax = new Vector2(segmentWidth, _secsBar.rectTransform.rect.height / 2);

            if (!activeSegmentSet && action == _queue.GetActiveCard())
            {
                _activeSegment = segmentObj;
                activeSegmentSet = true;
            }

            cumulativeTime += action._duration;
        }
    }

    void UpdateActiveSegment()
    {
        if (_activeSegment != null)
        {
            var activeCard = _queue.GetActiveCard();
            if (activeCard != null)
            {
                float remainingTime = activeCard.GetRemainingTime();
                float cumulativeTime = _queue.GetCumulativeTimeOfPreviousCards() + remainingTime;

                RectTransform rectTransform = _activeSegment.GetComponent<RectTransform>();
                rectTransform.anchorMax = new Vector2(cumulativeTime / _queue._MaxTimeBuffer, 0.5f); // Update width based on remaining time
                rectTransform.offsetMax = new Vector2(rectTransform.rect.width, _secsBar.rectTransform.rect.height / 2); // Adjust height if needed
            }
        }
    }
}
