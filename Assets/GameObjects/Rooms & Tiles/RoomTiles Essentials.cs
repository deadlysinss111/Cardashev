using System.Collections.Generic;

// Determines what Zone the Tileset is based on
public enum ZoneType
{
    Debug = -1
}


// Determines which tile prefab is at the location
public enum TileType
{
    Nothing = 0,
    Flat,
    Slope,
}

/*
 ! Only useful for when we'll want to compose a room of multiple prefabs
 • Contains essential data for loading prefab of rooms both in the scene and in the Tileset class
*/
public struct RoomPrefabDesc
{
    // Both of these are in Units (or length of a simple square tile)
    public int RoomW { get; }
    public int RoomH { get; }

    public RoomPrefabDesc(int ARGw, int ARGh)
    {
        RoomW = ARGw;
        RoomH = ARGh;
    }
}


/*
 • Contains a RoomPrefabStruct for each actual prefab of rooms we have
*/
public struct RoomPrefabEncyclopedia
{
    public Dictionary<ZoneType, string> ZoneFolderName { get; }
    // ! TODO: Implement when we'll want to compose a room of multiple prefabs
    public Dictionary<string, RoomPrefabDesc> RoomBook { get; }

    public RoomPrefabEncyclopedia(Dictionary<ZoneType, string> ARGZoneFolderName, Dictionary<string, RoomPrefabDesc> ARGRoomBook)
    {
        ZoneFolderName = ARGZoneFolderName;
        RoomBook = ARGRoomBook;
    }
}