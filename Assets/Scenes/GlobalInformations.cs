using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GlobalInformations
{
    // Container GameObjects
    public static GameObject[] _persistentSceneContainers = new GameObject[3];

    // Data needed for scene transition (Map <-> Room)
    static public string _prefabToLoadOnRoomEnter;
    static public List<List<GameObject>> _mapNodes; // TODO check for removal



    // Misc stuff to avoid aftternoons of "WHY ? WHY ? WHYYYYYY ? oh that's why."
    public enum PersistentSceneContainer
    {
        UNKNOWN = -1,
        Map,
        Room,
        EscMenu
    }


    /*
     METHODS
    */
    static public bool IsSceneContainerNull(PersistentSceneContainer ARGsceneContainerID)
    {
        // Target of the verification
        GameObject checkTarget = _persistentSceneContainers[ARGsceneContainerID];

        // Translates the string into a target for the verification
        switch (ARGsceneContainerID)
        {
            case PersistentSceneContainer.Map:
                checkTarget = _mapSceneContainer;
                break;
            case PersistentSceneContainer.Room:
                checkTarget = _roomSceneContainer;
                break;
            case PersistentSceneContainer.EscMenu:
                checkTarget = _escapeMenuSceneContainer;
                break;

            default:
                checkTarget = null;
                Debug.Log("GlobalInformations => IsSceneContainerNull didn't find a matching string !\nError incoming, take coveeeeeer !!! >:O");
        }

        // Performs the check
        if (checkTarget == null)
            return true;
        else
            return false;
    }

    // Translates a string into an enum memeber
    static public PersistentSceneContainer StringtoEnum(string ARGsceneContainerName)
    {
        switch (ARGsceneContainerName)
        {
            case "Map":
                return PersistentSceneContainer.Map;
                
            case "Room":
                return PersistentSceneContainer.Room;
                
            case "Esc Menu":
                return PersistentSceneContainer.EscMenu;

            default:
                return PersistentSceneContainer.UNKNOWN;
        }
    }
}
