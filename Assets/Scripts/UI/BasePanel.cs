using UnityEngine;

namespace UI
{
    public abstract class BasePanel <T> : MonoBehaviour
    {
        public T Data
        {
            get => _data;
        }

        private T _data;

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
        protected virtual void OnOpen(){}
        protected virtual void OnClose(){}
    }
}