using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;

    private static object _lock = new object();

    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                "' already destroyed on application quit." +
                " Won't create again - returning null.");
                return null;
            }

            lock (_lock)
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                        " - there should never be more than 1 singleton!" +
                        " Reopenning the scene might fix it.");
                        return instance;
                    }

                    if (instance == null)
                    {
                        GameObject singleton = new GameObject();
                        singleton.name = typeof(T).Name;
                        instance = singleton.AddComponent<T>();
                    }
                }

                return instance;
            }
        }
    }
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    protected void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
