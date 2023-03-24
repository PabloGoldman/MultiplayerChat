using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    private static MonoBehaviourSingleton<T> instance = null;

    public static T Instance
    {
        get 
        {
            if (instance == null)
                instance = FindObjectOfType<MonoBehaviourSingleton<T>>();

            return (T)instance;
        }
    }

    protected virtual void Initialize()
    {

    }

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);

        instance = this;

        Initialize();
    }
}
