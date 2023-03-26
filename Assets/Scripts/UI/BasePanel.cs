using System;
using UnityEngine;

namespace UI
{
    public abstract class BasePanel<T> : MonoBehaviour where T : IUpdateable
    {
        public T Data
        {
            get => _data;
            set => SetData(value);
        }

        private T _data;
        

        private void SetData(T d)
        {
            if (_data != null)
            {
                RemoveListener();
            }
            _data = d;

            AddListener();
            
            UpdateUI();
        }


        private void AddListener()
        {
            if (_data == null)
            {
                return;
            }
            _data.OnUpdated += UpdateUI;
            _data.OnDestroy -= UpdateUI;
        }

        private void RemoveListener()
        {
            _data.OnUpdated -= UpdateUI;
            _data.OnDestroy -= RemoveListener;
        }

        protected abstract void UpdateUI();
        
        protected virtual void OnOpen(){}
        protected virtual void OnClose(){}
    }
}