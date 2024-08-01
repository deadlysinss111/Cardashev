using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DixAnimator : MonoBehaviour
{
    [SerializeField] List<RectTransform> _dust;
    [SerializeField] List<RectTransform> _cross;

    void Update()
    {
        foreach (var dust in _dust)
        {
            dust.Translate(0.01f, 0, 0);
            if (dust.localPosition.x >= dust.rect.width*3)
            {
                dust.localPosition = new Vector2(-dust.rect.width, dust.localPosition.y);
            }
        }
        foreach (var cross in _cross)
        {
            cross.Translate(0.04f, -0.04f, 0);
            if (cross.localPosition.y <= -2000)
            {
                cross.localPosition = new Vector2(cross.localPosition.x - 4000, cross.localPosition.y+4000);
            }
        }
        
    }
}
//660, 600
//-2650, -530