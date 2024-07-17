using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEPreviewScript : MonoBehaviour
{
    public LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }
}
