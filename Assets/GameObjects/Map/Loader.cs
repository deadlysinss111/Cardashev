using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    /* 
     MISC
    */


    /* 
     FIELDS
    */
    // Nothing yet


    /* 
     PROPERTIES
    */



    /* 
     EVENTS
    */
    public UnityEvent<string, string> _UeSceneChange;


    /* 
     METHODS
    */
    // Scene loader
    public void LoadScene(string ARGcurScene, string ARGtargetScene)
    {
        // Check if it is the first time that this scene is loaded
        if (GlobalInformations._persistent)
    }


    // Scene unloader
    // Needs to deactivate a parent whilst saving some things in GlobalInformations








    public void Awake()
    {
        // Event subscribing
        _UeSceneChange.AddListener(LoadScene);
    }


    
    // ~~~~~~
    // Old code to load the map only

    [SerializeField] GameObject _map;

    void Start()
    {
        if (GlobalInformations._map == null)
        {
            GlobalInformations._map = Instantiate(_map);
            DontDestroyOnLoad(GlobalInformations._map);
        }
        else
        {
            GlobalInformations._map.SetActive(true);
        }
    }

    // Old code to load the map only
    // ~~~~~~
    */

}
