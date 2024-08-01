using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public sealed class ScriptableObjectUtils
{
    private ScriptableObjectUtils() { }

    public static T CreateAsset<T>(string name) where T:ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if(string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }
        else if(Path.GetExtension(path)!="")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }
        AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;

        return asset;
    }

    public static T GetOrCreateAsset<T>(string assetPath, string assetName, bool activeAsset = false) where T : ScriptableObject
    {
        string fullAssetPath = Path.Combine(assetPath, assetName + ".asset");
        var scriptabelObject = AssetDatabase.LoadAssetAtPath<T>(fullAssetPath);

        if(!scriptabelObject)
        {
            scriptabelObject = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(scriptabelObject, fullAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if(activeAsset) Selection.activeObject = scriptabelObject;

        return scriptabelObject;
    }
}
