using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using Unity.Mathematics;

// GI stands for GlobalInformations
static public class GI
{
    [SerializeField] public static GameObject _mapPrefab;

    // Misc stuff to avoid afternoons of "WHY ? WHY ? WHYYYYYY ? oh that's why."
    // When adding elements, never put any `=` anywhere or this will break the below array
    public enum PersistentSceneContainer
    {
        NON_PERSISTENT = -1,
        Map,
    }

    // Array that stores prefabs for each

    // Array that stores persistent GameObjects. Its size automatically fits the Enum size
    public static GameObject[] _persistentSceneContainers = new GameObject[Enum.GetNames(typeof(PersistentSceneContainer)).Length - 1];

    // Data needed for scene transition (Map --> Room)
    static public string _prefabToLoadOnRoomEnter;
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
