using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem.Android;

// GI stands for GlobalInformations
static public class GI
{
    // ------
    // COMMON FETCHERS
    // ------

    // Components needed by a LOT of MonoBehaviours
    static PlayerManager _playerManager;
    static StatManager _playerStat;
    static GameObject _player;
    static public Loader _loader;
    static public GameObject _deckContainer;

    // Super cool Funcs to treat the above Components as a Singleton, limiting calls of GameObject.Find() everywhere :D
    static public Func<PlayerManager> _PManFetcher;

    static public Func<GameObject> _PlayerFetcher;

    static public Func<StatManager> _PStatFetcher;


    // ------
    // PERSISTENT SCENES
    // ------

    // Misc stuff to avoid afternoons of "WHY ? WHY ? WHYYYYYY ? oh that's why.". Never add any `=`, or the array WILL break
    public enum PersistentSceneContainer
    {
        NON_PERSISTENT = -1,
        Map,
        EntryPoint,
    }

    // Array that stores persistent GameObjects. Its size automatically fits the Enum's size
    public static GameObject[] _persistentSceneContainers = new GameObject[Enum.GetNames(typeof(PersistentSceneContainer)).Length - 1];

    // Data needed for scene transition (Map --> Room)
    static public string _roomType;

    static public bool _changeStateOnHUDExit = false;
    static public bool temp { set { _changeStateOnHUDExit = value; } }


    // ------
    // SCENE LOADING UTILS
    // ------

    // Dict to chnage in one place if a fcking scene undergoes name change
    static public Dictionary<string, string> _SceneNameEncyclopedia = new Dictionary<string, string>{
        { "", "" },
        { "EntryPoint", "EntryPoint" },
        { "Map", "MapNavigation" },
        { "Room", "TestLvl" },
        { "Reward", "RewardScene" },
        { "MainMenu", "MenuScene"}};

    // UnityEvents on scene load
    static public UnityEngine.Events.UnityEvent _UeOnMapSceneLoad = new();
    static public List<UnityEngine.Events.UnityEvent> _SceneLoadUEventList = new List<UnityEngine.Events.UnityEvent> { _UeOnMapSceneLoad };


    // <~~[ UNSURE ]~~>

    static public GameObject _map; // maybe not correctly written (should be an array ?)
    static public float _gameTimer;

    static public string _cursorVoid = null;
    static public string _cursorTiles = null;
    static public string _cursorEnemies = null;
    static public string _cursorInteractibles = null;
    static public string _cursorSTiles = null;
    static public string _cursorSEnemies = null;
    static public string _cursorSInteractibles = null;

    public enum CursorRestriction
    {
        VOID = 1,
        TILES = 2,
        ENEMIES = 4,
        INTERACTIBLES = 8,
        S_TILES = 16,
        S_ENEMIES = 32,
        S_INTERACTIBLES = 64,
    }

    /*
     METHODS
    */
    // 2 Methods to give information to the loader
    static public bool IsPersistentSceneContainerNull(string ARGsceneContainerName)
    {
        //PersistentSceneContainer sceneID = SceneNametoEnum(_SceneNameEncyclopedia.FirstOrDefault(x => x.Value == ARGsceneContainerName).Key);
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

            case "EntryPoint":
                return PersistentSceneContainer.EntryPoint;

            default:
                Debug.LogWarning("SceneNametoEnum() found object to be NON_PERSISTENT.\nMaybe the switch case is incomplete ?");
                return PersistentSceneContainer.NON_PERSISTENT;
        }
    }

    // Instantiates and saves a persistent scene container GameObject
    static public void InstantiateAndCull(string ARGsceneContainerName)
    {
        // Pointer to the array's cell corresponding to the persistent scene. This avoids doing the big ass array access below 3 times
        GameObject sceneContainer = _persistentSceneContainers[ (int) SceneNametoEnum(_SceneNameEncyclopedia.FirstOrDefault(x => x.Value == ARGsceneContainerName).Key) ];
        switch (_SceneNameEncyclopedia.FirstOrDefault(x => x.Value == ARGsceneContainerName).Key)
        {
            case "EntryPoint":
                // Instantiate, save and cull
                _map = GameObject.Find("Map");
                //sceneContainer = MonoBehaviour.Instantiate(_mapPrefab);
                MonoBehaviour.DontDestroyOnLoad(_map);
                _map.SetActive(false);

                _deckContainer = GameObject.Find("DeckContainer");
                MonoBehaviour.DontDestroyOnLoad(_deckContainer);
                break;

            default:
                Debug.LogError("how TF did you get an error in there ? õ_Ô");
                break;
        }
    }
    
    static public void Uncull(string ARGsceneContainerName)
    {
        // Pointer to the array's cell corresponding to the persistent scene. This avoids doing the big ass array access below 3 times
        GameObject sceneContainer = _persistentSceneContainers[ (int) SceneNametoEnum(_SceneNameEncyclopedia.FirstOrDefault(x => x.Value == ARGsceneContainerName).Key) ];
        switch (_SceneNameEncyclopedia.FirstOrDefault(x => x.Value == ARGsceneContainerName).Key)
        {
            case "Map":
                // Instantiate, save and cull
                _map.SetActive(true);
                break;

            default:
                Debug.LogError("how TF did you get an error in there ? õ_Ô");
                break;
        }
    }


    static public void ResetFetchers()
    {
        _PManFetcher = () => {
            _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
            if (_playerManager != null)
            {
                _PManFetcher = () => { return _playerManager; };
                return _playerManager;
            }
            Debug.LogError("It's so sad PManFetcher died of ligma");
            return null;
        };

        _PlayerFetcher = () => {
            _player = GameObject.Find("Player");
            if (_player != null)
            {
                _PlayerFetcher = () => { return _player; };
                return _player;
            }
            Debug.LogError("Me when the PlayerFetcher is bad at the game ong ō_ō");
            return null;
        };

        _PStatFetcher = () => {
        if (GameObject.Find("Player").TryGetComponent(out _playerStat))
        {
            _PStatFetcher = () => { return _playerStat; };
            return _playerStat;
        }
        Debug.LogError("Bro is PStatFetcherless :skullemoji:");
        return null;
        };
    }

    //Change the cursor image to the one that is allocated to the argument sate
    static public void SetCursorTo(CursorRestriction target)
    {
        switch (target)
        {
            case CursorRestriction.S_INTERACTIBLES:
                SetCursorTo(_cursorSInteractibles);
                break;
            case CursorRestriction.S_ENEMIES:
                SetCursorTo(_cursorSEnemies);
                break;
            case CursorRestriction.S_TILES:
                SetCursorTo(_cursorSTiles);
                break;
            case CursorRestriction.INTERACTIBLES:
                SetCursorTo(_cursorInteractibles);
                break;
            case CursorRestriction.ENEMIES:
                SetCursorTo(_cursorSEnemies);
                break;
            case CursorRestriction.TILES:
                SetCursorTo(_cursorTiles);
                break;
            case CursorRestriction.VOID:
                SetCursorTo(_cursorVoid);
                break;
        }
    }

    // Internal methode that has no restiction. The restiction on the previous function is meant to make it easier to understand.
    static private void SetCursorTo(string target)
    {
        Cursor.SetCursor((Texture2D)Resources.Load(target), Vector2.zero, CursorMode.ForceSoftware);
    }

    // Allocate a new image to the state(s) you passes as arguments
    // Follow the following exemple for passing multiple states as argument
    // UpdateCursor("myCursorImg", (byte)CursorRestriction.VOID + (byte)ENEMIES)
    static public void UpdateCursors(string name, byte restriction)
    {
        byte counter = 64;
        while (counter > 0)
        {
            if (restriction - counter >= 0)
            {
                restriction -= counter;
                switch (counter)
                {
                    case (byte)CursorRestriction.S_INTERACTIBLES:
                        _cursorSInteractibles = name;
                        break;
                    case (byte)CursorRestriction.S_ENEMIES:
                        _cursorSEnemies = name;
                        break;
                    case (byte)CursorRestriction.S_TILES:
                        _cursorSTiles = name;
                        break;
                    case (byte)CursorRestriction.INTERACTIBLES:
                        _cursorInteractibles = name;
                        break;
                    case (byte)CursorRestriction.ENEMIES:
                        _cursorEnemies = name;
                        break;
                    case (byte)CursorRestriction.TILES:
                        _cursorTiles = name;
                        break;
                    case (byte)CursorRestriction.VOID:
                        _cursorVoid = name;
                        break;
                }
            }
            counter /= 2;
        }
    }

    // Allocate a new image to all the states, except those you passes as arguments
    // Follow the following exemple for passing multiple states as argument
    // UpdateCursor("myCursorImg", (byte)CursorRestriction.VOID + (byte)ENEMIES)
    static public void UpdateCursorsInverted(string name, int restriction)
    {
        int counter = 64;
        while (counter > 0)
        {
            if (restriction - counter < 0)
            {
                switch (counter)
                {
                    case (byte)CursorRestriction.S_INTERACTIBLES:
                        _cursorSInteractibles = name;
                        break;
                    case (byte)CursorRestriction.S_ENEMIES:
                        _cursorSEnemies = name;
                        break;
                    case (byte)CursorRestriction.S_TILES:
                        _cursorSTiles = name;
                        break;
                    case (byte)CursorRestriction.INTERACTIBLES:
                        _cursorInteractibles = name;
                        break;
                    case (byte)CursorRestriction.ENEMIES:
                        _cursorEnemies = name;
                        break;
                    case (byte)CursorRestriction.TILES:
                        _cursorTiles = name;
                        break;
                    case (byte)CursorRestriction.VOID:
                        _cursorVoid = name;
                        break;
                }
            }
            else
            {
                if (counter == 0) return;
                restriction -= counter;
            }
            counter /= 2;
        }
    }

    // Reset the cursors to the default one for each states
    static public void ResetCursorValues()
    {
        _cursorSInteractibles = null;
        _cursorSEnemies = null;
        _cursorSTiles = null;
        _cursorInteractibles = null ;
        _cursorSEnemies = null;
        _cursorTiles = null;
        _cursorVoid = null;
    }
}
