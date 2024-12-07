using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.IO;

public class Loader : MonoBehaviour
{
    // Use the keys to act on .unity files. Make sure the associated values are up-to-date with the in-project file names !
    //private Dictionary<string, string> _sceneEncylopedia = new Dictionary<string, string> {
    //    { "", "" },
    //    { "Map", "MapNavigation" },
    //    { "Room", "TestLvl" },
    //    { "MainMenu", "MenuScene" }};

    private static RoomPrefabEncyclopedia ROOM_ENCYCLOPEDIA = new RoomPrefabEncyclopedia
    (
        new Dictionary<ZoneType, string> {
            { ZoneType.Debug, "Debug" },
            { ZoneType.Radioactive, "Radioactive" },
        },

        new Dictionary<string, RoomPrefabDesc> { }
    );

    private ZoneType _zoneType = ZoneType.Radioactive;

    /* 
     EVENTS
    */


    /* 
     METHODS
    */
    public void Awake()
    {
        // Ensure the object is always there
        DontDestroyOnLoad(this);

        // Give its reference to GI
        GI._loader = this;

        GI._UeOnMapSceneLoad.AddListener(GI.ResetFetchers);
    }

    public void Start()
    {
        // Loads the first scene
        LoadScene("EntryPoint", "MainMenu");
    }


    // Scene loader
    public void LoadScene(string ARGcurScene, string ARGtargetScene, LoadSceneMode ARGloadMode = LoadSceneMode.Single)
    {
        string curScene = ARGcurScene;
        string targetScene = ARGtargetScene;
        if (GI._SceneNameEncyclopedia.ContainsKey(ARGcurScene))
            curScene = GI._SceneNameEncyclopedia[ARGcurScene];
        if (GI._SceneNameEncyclopedia.ContainsKey(ARGtargetScene))
            targetScene = GI._SceneNameEncyclopedia[ARGtargetScene];

        // Check if the scene we are leaving is persistent (if it is, we need to do more work)
        if (true == GI.IsSceneContainerPersistent(curScene) && ARGloadMode == LoadSceneMode.Single)
        {
            // Check if the persistent scene was never "saved" before, and saves it if not
            if (true == GI.IsPersistentSceneContainerNull(curScene))
                GI.InstantiateAndCull(curScene);
            else
                GI._persistentSceneContainers[ (int) GI.SceneNametoEnum(curScene) ].SetActive(false);
        }

        SceneManager.LoadScene(targetScene, ARGloadMode);

        if (true == GI.IsSceneContainerPersistent(ARGtargetScene) && ARGloadMode == LoadSceneMode.Single)
        {
            // Check if the persistent scene was never "saved" before, and saves it if not
            //if (true == GI.IsPersistentSceneContainerNull(ARGtargetScene))
            GI.Uncull(targetScene);
        }

        // By this points, everything should be saved or culled, so let's change the scene :3

        // This coroutine will Invoke events related to Load Scene, waiting for all Awake() of the loaded scene to resolve
        StartCoroutine(SceneLoadInvoker());
    }

    // Coroutine that waits the execution of every Awake() after a scene load to then raise events of a scene loading
    IEnumerator SceneLoadInvoker()
    {
        // Wait an amount of ticks before carrying on
        for (int i = 0;  i < 1;  ++i)
            yield return null;

        //   Raises scene load events
        // ! This MIGHT cause issues in the future, idrk
        foreach (UnityEvent UEvent in GI._SceneLoadUEventList)
            UEvent.Invoke();
    }

    public void LoadRoom(string roomType)
    {
        string path = "Assets/GameObjects/Rooms & Tiles/Resources/" + ROOM_ENCYCLOPEDIA.ZoneFolderName[_zoneType] + " Zone/RoomPrefabs/" + GI._roomType;
        int metaFilesAmount = Directory.GetFiles(path, "*.meta", SearchOption.TopDirectoryOnly).Length;
        int size = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Length;
        size -= metaFilesAmount;
        //print("size is " + size);
        SceneManager.LoadScene(ROOM_ENCYCLOPEDIA.ZoneFolderName[_zoneType] + GI._roomType + UnityEngine.Random.Range(1, size+1).ToString(), LoadSceneMode.Single);
        GI.UpdateMapState();
    }

    /// <summary>
    /// Removes the objects in DontDestroyOnLoad and returns to the EntryPoint scene
    /// </summary>
    public void SoftRestart()
    {
        foreach (var go in gameObject.scene.GetRootGameObjects())
        {
            if (go.name.Contains("Debug") == false && go != this.gameObject)
            {
                DestroyImmediate(go);
            }
        }
        DestroyImmediate(gameObject);
        CurrentRunInformations.Reset();
        GI.ResetCursorValues();
        GI.ResetData();
        Time.timeScale = 1f;
        SceneManager.LoadScene("EntryPoint", LoadSceneMode.Single);
    }
}
