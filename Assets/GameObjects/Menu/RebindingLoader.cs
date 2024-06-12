using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindingLoader : MonoBehaviour
{
    public InputActionAsset _inputActions;

    public void OnEnable()
    {
        string rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
        {
            _inputActions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    public void OnDisable()
    {
        string rebinds = _inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
}
