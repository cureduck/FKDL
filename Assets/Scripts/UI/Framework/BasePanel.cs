using Game;
using Managers;
using UnityEngine;

namespace UI
{
    public interface IUIPanel<in T>
    {
        void Open(T data);
    }

    public abstract class BasePanel<T> : MonoBehaviour
    {
        private T _data;
        protected static PlayerData PlayerData => GameManager.Instance.PlayerData;

        public bool IsOpen => gameObject.activeInHierarchy;

        public T Data
        {
            get => _data;
        }

        public virtual void Init()
        {
        }

        public virtual void Open(T data)
        {
            SetData(data);
            OnOpen();
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            OnClose();
            gameObject.SetActive(false);
        }

        protected virtual void SetData(T d)
        {
            _data = d;


            UpdateUI();
        }

        protected abstract void UpdateUI();

        protected virtual void OnOpen()
        {
        }

        protected virtual void OnClose()
        {
        }
    }
}