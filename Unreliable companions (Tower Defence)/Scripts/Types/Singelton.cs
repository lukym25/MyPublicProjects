using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lukas.MyClass
{
    public class MySingelton<T> : MonoBehaviour
    where T : Component
    {
        private static T _instance;
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    T[] objects = FindObjectsOfType(typeof(T)) as T[];
                    if (objects.Length > 0)
                    {
                        _instance = objects[0];
                    }
                    else
                    {
                        GameObject obj = new GameObject();
                        _instance = obj.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

    }
}
