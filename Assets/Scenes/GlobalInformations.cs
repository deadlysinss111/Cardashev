using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// GI stands for GlobalInformations
static public class GI
{
    // ------
    // COMMON FETCHERS
    // ------

    // Components needed by a LOT of MonoBehaviours
    static public PlayerManager _playerManager;
    static public GameObject _player;

    // Super cool Funcs to treat the above Components as a Singleton, limiting calls of GameObject.Find() everywhere :D
    static public Func<PlayerManager> _PManFetcher = () => {
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        if (_playerManager != null)
        {
            _PManFetcher = () => { return _playerManager; };
            return _playerManager;
        }
        Debug.LogError("It's so sad PManFetcher died of ligma");
        return null;
    };
    static public Func<GameObject> _PlayerFetcher = () => {
        _player = GameObject.Find("Player");
        if (_player != null)
        {
            _PlayerFetcher = () => { return _player; };
            return _player;
        }
        Debug.LogError("Me when the PlayerFetcher is bad at the game ong ō_ō");
        return null;
    };


    // ------
    // PERSISTENT SCENES
    // ------

    // Misc stuff to avoid afternoons of "WHY ? WHY ? WHYYYYYY ? oh that's why.". Never add any `=`, or the array WILL break
    public enum PersistentSceneContainer
    {
        NON_PERSISTENT = -1,
        Map,
    }

    // Array that stores persistent GameObjects. Its size automatically fits the Enum's size
    public static GameObject[] _persistentSceneContainers = new GameObject[Enum.GetNames(typeof(PersistentSceneContainer)).Length - 1];

    // Data needed for scene transition (Map --> Room)
    static public string _prefabToLoad;
    static public List<List<GameObject>> _mapNodes; // TODO check for removal


    // ------
    // SCENE LOADING UTILS
    // ------

    // Dict to chnage in one place if a fcking scene undergoes name change
    static public Dictionary<string, string> _SceneNameEncyclopedia = new Dictionary<string, string>{
        { "Map", "MapNavigation" },
        { "Room", "TestLvl" },
        { "MainMenu", "MenuScene"}};

    // UnityEvents on scene load
    static public UnityEngine.Events.UnityEvent _UeOnMapSceneLoad = new();
    static public List<UnityEngine.Events.UnityEvent> _SceneLoadUEventList = new List<UnityEngine.Events.UnityEvent> { _UeOnMapSceneLoad };


    // <~~[ UNSURE ]~~>

    static public GameObject _mapPrefab; // maybe not correctly written (should be an array ?)
    static public float _gameTimer;


    /*
     METHODS
    */
    // 2 Methods to give information to the loader
    static public bool IsPersistentSceneContainerNull(string ARGsceneContainerName)
    {
        PersistentSceneContainer sceneID = SceneNametoEnum(ARGsceneContainerName);

        // Performs the check
        if (_persistentSceneContainers[ (int)sceneID ] == null)
            return true;
        else
            return false;
    }
    static public bool IsSceneContainerPersistent(string ARGsceneContainerName)
    {
        PersistentSceneContainer sceneID = SceneNametoEnum(ARGsceneContainerName);

        // Performs the check
        if (sceneID == PersistentSceneContainer.NON_PERSISTENT)
            return false;
        else
            return true;
    }

    // Translates a string into an enum member. Don't forget any !
    static public PersistentSceneContainer SceneNametoEnum(string ARGsceneContainerName)
    {
        switch (ARGsceneContainerName)
        {
            case "Map":
                return PersistentSceneContainer.Map;

            default:
                Debug.LogWarning("SceneNametoEnum() found object to be NON_PERSISTENT.\nMaybe the switch case is incomplete ?");
                return PersistentSceneContainer.NON_PERSISTENT;
        }
    }

    // Instantiates and saves a persistent scene container GameObject
    static public void InstantiateAndCull(string ARGsceneContainerName)
    {
        // Pointer to the array's cell corresponding to the persistent scene. This avoids doing the big ass array access below 3 times
        GameObject sceneContainer = _persistentSceneContainers[ (int) SceneNametoEnum(ARGsceneContainerName) ];
        
        switch (ARGsceneContainerName)
        {
            case "Map":
                // Instantiate, save and cull
                sceneContainer = MonoBehaviour.Instantiate(_mapPrefab);
                MonoBehaviour.DontDestroyOnLoad(sceneContainer);
                sceneContainer.SetActive(false);
                break;

            default:
                Debug.LogError("how TF did you get an error in there ? õ_Ô");
                break;
        }
    }
}
