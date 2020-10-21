using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
#endif

/// <summary>
/// Singleton que sea auto instancia e inicializa dentro de la carpeta Resources
/// </summary>
/// <typeparam name="T">Referencia circular a la propia clase de la que se quiere hacer Singleton</typeparam>
public abstract class RuntimeScriptableSingleton<T> : BaseRuntimeScriptableSingleton where T : RuntimeScriptableSingleton<T>
{
    
#if UNITY_EDITOR
    public static class UpdateGit
    {
        [MenuItem("Window/Ishimine/Update/ScriptableSettings")]
        public static void SelectMe() => Client.Add("https://github.com/FelipeIshimine/RuntimeScriptableSingleton.git");
    }
#endif
    
    
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;
            try
            {
                _instance = Resources.LoadAll<T>("")[0];
            }
            catch (System.Exception error)
            {
                Debug.Log(error);
#if UNITY_EDITOR
                _instance = CreateInstance<T>();
                AssetDatabase.CreateAsset(_instance, ResourcesPath + typeof(T).Name + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
            }
            _instance.InitializeSingleton();
            return _instance;
        }
        set
        {
            _instance = value;
            Debug.Log(" <Color=green> SCRIPTABLE_SINGLETON Initialized: </color> <Color=blue> " + _instance + "</color> ");
        }
    }

    public static string ResourcesPath => "Assets/Resources/";
    public virtual string FileName =>  typeof(T).Name;
    public virtual string FilePath => ResourcesPath + FileName + ".asset";

    public T Myself => this as T;

    public override void InitializeSingleton() { }
}

public abstract class BaseRuntimeScriptableSingleton : ScriptableObject
{
    public abstract void InitializeSingleton();
}