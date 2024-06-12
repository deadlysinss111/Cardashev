using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeybindManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public InputActionAsset inputActions;
    public GameObject keybindUIContainer;   // A parent GameObject containing the UI elements
    public GameObject keybindUIPrefab;      // A prefab for each keybinding UI element

    private Dictionary<string, Text> actionNameToTextMap;

    private void Awake()
    {
        LoadBindings();
        SetupKeybindUI();
    }

    private void SetupKeybindUI()
    {
        actionNameToTextMap = new Dictionary<string, Text>();

        foreach (InputActionMap actionMap in inputActions.actionMaps)
        {
            foreach (InputAction action in actionMap.actions)
            {
                GameObject bindingDisplay = Instantiate(keybindUIPrefab, keybindUIContainer.transform);
                Text[] texts = bindingDisplay.GetComponentsInChildren<Text>();

                if (texts.Length >= 2)
                {
                    texts[0].text = action.name; // Set action name
                    actionNameToTextMap[action.name] = texts[1]; // Reference to binding display text
                    DisplayCurrentBinding(action.name);
                }
            }
        }
    }

    public void StartRebinding(string actionName, int bindingIndex)
    {
        InputAction action = inputActions.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError($"Action '{actionName}' not found.");
            return;
        }

        actionNameToTextMap[actionName].text = "Press a key...";

        InputActionRebindingExtensions.RebindingOperation rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                operation.Dispose();
                DisplayCurrentBinding(actionName);
                SaveBindings();
            })
            .Start();
    }

    private void DisplayCurrentBinding(string actionName)
    {
        InputAction action = inputActions.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError($"Action '{actionName}' not found.");
            return;
        }

        InputBinding binding = action.bindings[0];
        actionNameToTextMap[actionName].text = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    public void SaveBindings()
    {
        string rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

    public void LoadBindings()
    {
        string rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
        {
            inputActions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    public void OnRebindButtonClicked(string actionName)
    {
        StartRebinding(actionName, 0); // Assuming single binding index for simplicity
    }
}
