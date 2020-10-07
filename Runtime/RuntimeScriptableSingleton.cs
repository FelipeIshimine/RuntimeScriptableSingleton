using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Singleton que sea auto instancia e inicializa dentro de la carpeta Resources
/// </summary>
/// <typeparam name="T">Referencia circular a la propia clase de la que se quiere hacer Singleton</typeparam>
public abstract class RuntimeScriptableSingleton<T> : BaseScriptableSingleton where T : RuntimeScriptableSingleton<T>
{
    public static List<BaseScriptableSingleton> RuntimeScriptableSingletons { get; private set; } = new List<BaseScriptableSingleton>();
    
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
            return _instance;
        }
        set
        {
            _instance = value;
            Debug.Log(" <Color=green> SCRIPTABLE_SINGLETON Initialized: </color> <Color=blue> " + _instance + "</color> ");
        }
    }

    public static string ResourcesPath => "Assets/Resources/";
    public virtual string FilePath => ResourcesPath + typeof(T).Name + ".asset";

    public abstract T Myself
    {
        get;
    }

    public override void InitializeSingleton()
    {
        if (_instance == null)
        {
            Instance = Myself;
            RuntimeScriptableSingletons.Add(this);
        }
        else if (_instance != this)
        {
            Debug.LogError("<Color=red> " + this + "  SCRIPTABLE_SINGLETON ALREADY EXIST CONFLICT </color>");
        }
    }

    protected virtual void OnDisable()
    {
        _instance = null;
    }
}

public abstract class BaseScriptableSingleton : ScriptableObject
{
    public abstract void InitializeSingleton();
}