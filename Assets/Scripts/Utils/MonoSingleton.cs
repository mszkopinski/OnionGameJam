using UnityEngine;

namespace Utils
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (instance != null) return instance;
                instance = (T)FindObjectOfType(typeof(T));
                return instance;
            }
        }

        static T instance;
    }
}