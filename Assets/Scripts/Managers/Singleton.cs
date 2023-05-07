using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    public abstract class Singleton<T> : SerializedMonoBehaviour where T : Singleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    // 查找是否已有本类型的实例
                    instance = FindObjectOfType<T>();

                    // 如果没有，则创建新实例并添加到场景中
                    if(instance == null)
                    {
                        GameObject obj = new GameObject();
                        instance = obj.AddComponent<T>();
                        obj.name = typeof(T).ToString();
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            // 确保单例只有一个实例
            if(instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = (T)this;
            }
        }
    }
}