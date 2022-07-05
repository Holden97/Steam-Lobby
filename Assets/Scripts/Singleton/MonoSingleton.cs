using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                if (GameObject.FindObjectsOfType<T>() != null && GameObject.FindObjectsOfType<T>().Length > 1)
                {
                    Debug.LogError("singleton class More than 1!");
                }
                if (GameObject.FindObjectOfType<T>() != null)
                {
                    instance = GameObject.FindObjectOfType<T>();
                }
                else
                {
                    GameObject root = new GameObject();
                    root.AddComponent<T>();
                    root.name = $"singleton_{nameof(T)}";
                    instance = root.GetComponent<T>();
                }

            }
            return instance;
        }
    }
}
