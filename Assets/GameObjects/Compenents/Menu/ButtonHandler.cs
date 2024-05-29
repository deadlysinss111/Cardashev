using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    bool mouse_on = false;

    Vector3 default_scales;

    // Start is called before the first frame update
    void Start()
    {
        default_scales = transform.localScale;

        // Changes "Start" text to "Continue" if there's a savefile available
        if (gameObject.name != "Start") return;
        TMP_Text text = transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        if (text is null) return;

        // TODO: Check for savefile
        if (true)
        {
            text.text = "Continue";
        }
    }

    private void Update()
    {
        Vector3 scale = transform.localScale;
        if (mouse_on)
        {
            float select_mult = GetComponentInParent<MainMenuManager>()._buttonsSelectMult;
            scale.x = Mathf.MoveTowards(scale.x, default_scales.x * select_mult, 0.15f);
            scale.y = Mathf.MoveTowards(scale.y, default_scales.y * select_mult, 0.15f);
            scale.z = Mathf.MoveTowards(scale.z, default_scales.z * select_mult, 0.15f);
        }
        else
        {
            scale.x = Mathf.MoveTowards(scale.x, default_scales.x, 0.15f);
            scale.y = Mathf.MoveTowards(scale.y, default_scales.y, 0.15f);
            scale.z = Mathf.MoveTowards(scale.z, default_scales.z, 0.15f);
        }
        transform.localScale = scale;
    }

    public void MouseEnter()
    {
        mouse_on = true;
    }

    public void MouseExit()
    {
        mouse_on = false;
    }
}
