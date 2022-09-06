using System;
using System.Reflection;
using Sirenix.OdinInspector;

namespace Managers
{
    public class Singleton <T> : SerializedMonoBehaviour where T: Singleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = (T) this;
            }
        }

        protected void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}