using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // By how much buttons will scale up when hovering them
    public float _buttonsSelectMult;

    public List<GameObject> _menuObjects;
    Dictionary<string, GameObject> _menus;
    string _currentMenu;

    private void Start()
    {
        _buttonsSelectMult = 1.2f;
        _menuObjects = new();
        _currentMenu = string.Empty;
        _menus = new();

        //_menuObjects.Clear();
        foreach (Transform child in transform)
            _menuObjects.Add(child.gameObject);

        // Create a dictionary based on the menu_objects list
        for (int i = 0; i < _menuObjects.Count; i++)
        {
            _menus[_menuObjects[i].name] = _menuObjects[i];
        }
        //BetterDebug.LogDict<string, GameObject>(_menus);

        SetMenu(_menuObjects[0].name);
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

    public void GoToCharacterSelection()
    {
        SetMenu("New Game");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void GoToOptions()
    {
        SetMenu("Options");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    ////////////////////
    /// OPTIONS MENU ///
    ////////////////////

    public void GoToMain()
    {
        SetMenu("Main");
    }

    public void SelectCharacter(string name)
    {
        /*
        Action init;
        ClassFactory._classesBook.TryGetValue(name, out init);
        init();
        SceneManager.LoadScene("MapNavigation");
        */
    }
}
