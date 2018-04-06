using System;
using UnityEngine;

namespace Commons.Singleton
{
    /// <summary>
    /// @author Mayank Grover
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance;

        public event Action OnEnabled;
        public event Action OnDisabled;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<T>();
                    if (instance == null)
                    {
                        Debug.LogError("Could not find MonoSingleton on Singleton Object : " + typeof(T).ToString());
                    }
                }

                return instance;
            }
            protected set
            {
                if (instance == null)
                {
                    instance = value;
                    DontDestroyOnLoad(instance);
                }
                else
                {
                    Debug.LogError("Can't set multiple values of a singleton instance for type: " + value.GetType().FullName);
                }
            }
        }

        protected virtual void Awake()
        {
            instance = (T)this;
            OnInitialized();
        }

        protected virtual void Start() { }

        public virtual void Enable()
        {
            gameObject.SetActive(true);
            if (OnEnabled != null)
            {
                OnEnabled();
            }
        }

        public virtual void Disable()
        {
            if (OnDisabled != null)
            {
                OnDisabled();
            }

            gameObject.SetActive(false);
        }

        public virtual void OnInitialized() { }
    }
}
