using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // By how much buttons will scale up when hovering them
    public float _buttonsSelectMult = 1.2f;

    public bool _useChildren = false;
    public List<GameObject> _menuObjects = new();

    readonly Dictionary<string, GameObject> _menus = new();
    string _currentMenu = string.Empty;

    private void Start()
    {
        if (_useChildren)
        {
            _menuObjects.Clear();
            foreach (Transform child in transform)
                _menuObjects.Add(child.gameObject);
        }

        // Create a dictionary based on the menu_objects list
        for (int i = 0; i < _menuObjects.Count; i++)
        {
            _menus[_menuObjects[i].name] = _menuObjects[i];
        }
        BetterDebug.LogDict<string, GameObject>(_menus);

        SetMenu(_menuObjects[0].name);
        Debug.Log(this.GetMenu("Options"));
        Debug.Log(this.GetMenu());

    }

    /// <summary>
    /// Gets the GameObject of the menu name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetMenu(string name)
    {
        if (_menus.ContainsKey(name) == false)
        {
            throw new KeyNotFoundException("The menu " + name + " does not exist.");
        }
        return _menus[name];
    }
    /// <summary>
    /// Gets the GameObject of the current menu
    /// </summary>
    /// <returns></returns>
    public GameObject GetMenu()
    {
        return _menus[_currentMenu];
    }

    /// <summary>
    /// Activates the menu name and deactivate every other one
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public void SetMenu(string name)
    {
        if (_menus.ContainsKey(name) == false)
        {
            throw new KeyNotFoundException("The menu "+name+" does not exist.");
        }
        foreach (var kvp in _menus)
        {
            kvp.Value.SetActive(kvp.Key == name);
        }
        _currentMenu = name;
    }

    /////////////////
    /// MAIN MENU ///
    /////////////////

    public void MainMenuStart()
    {
        Debug.Log("Start");
        SceneManager.LoadScene("SampleScene");
    }

    public void MainMenuOptions()
    {
        Debug.Log("Options");
        SetMenu("Options");
    }

    public void MainMenuQuit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
        Debug.Log("Quit");
    }

    ////////////////////
    /// OPTIONS MENU ///
    ////////////////////

    public void OptionsMenuBack()
    {
        Debug.Log("Options Back Button");
        SetMenu("Main");
    }
}
