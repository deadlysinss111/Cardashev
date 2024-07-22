using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class VoxelLoader : MonoBehaviour
{
    [MenuItem("Tools/Load Voxel File")]
    public static void Load()
    {
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        EditorSceneManager.SaveScene(newScene, "Assets/LoadVoxTests/myScene.unity");
        
        foreach(Voxel voxel in VoxelReader.Read("Assets/LoadVoxTests/src.vox"))
        {
            GameObject prefab = FindAdaptedPrefab(voxel);
            if(prefab != null)
            {
                GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, newScene);
                prefabInstance.transform.position = new Vector3(voxel.x, voxel.z, voxel.y);
                print(":)");
            }
        }
        print("yup");
    }

    private static GameObject FindAdaptedPrefab(Voxel vox)
    {
        GameObject found = null;
        switch(vox.colorIndex)
        {
            case 250:
                found = AssetDatabase.LoadAssetAtPath<GameObject>("Assets\\Art\\test import\\tiles\\tile block\\rouille\\Prefab_Tile_block_rouille.prefab");
                break;
            default:
                print("you probably missed somewhere with color index : "+vox.colorIndex);
                break;
        }
        return found;
    }
}
