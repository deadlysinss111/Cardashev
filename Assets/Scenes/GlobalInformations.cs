using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GI stands for GlobalInformations
static public class GI
{
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

    static public GameObject _mapPrefab; // maybe not correctly written (should be an array ?)
    static public float _gameTimer;

    // Misc stuff to avoid afternoons of "WHY ? WHY ? WHYYYYYY ? oh that's why."
    // When adding elements, never put any `=` anywhere or this will break the below array
    public enum PersistentSceneContainer
    {
        NON_PERSISTENT = -1,
        Map,
    }

    // Array that stores persistent GameObjects. Its size automatically fits the Enum size
    public static GameObject[] _persistentSceneContainers = new GameObject[Enum.GetNames(typeof(PersistentSceneContainer)).Length - 1];

    // Data needed for scene transition (Map --> Room)
    static public string _prefabToLoad;
    static public List<List<GameObject>> _mapNodes; // TODO check for removal


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
                Debug.Log("SceneNametoEnum() found object to be NON_PERSISTENT. Maybe the switch case is incomplete ?");
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
                Debug.Log("how TF did you get an error in there ? õ_Ô");
                break;
        }
    }
}
