using System;
using UnityEngine;

namespace UI
{
    public abstract class UpdateablePanel<T> : BasePanel<T> where T : IUpdateable
    {
        protected override void SetData(T d)
        {
            if (_data != null)
            {
                RemoveListener();
            }
            base.SetData(d);
            AddListener();
            UpdateUI();
        }


        public override void Close()
        {
            RemoveListener();
            base.Close();
        }


        private void AddListener()
        {
            if (_data != null) 
            {
                _data.OnUpdated += UpdateUI;
                _data.OnDestroy += RemoveListener;
            }
        }

        private void RemoveListener()
        {
            if (_data != null)
            {
                _data.OnUpdated -= UpdateUI;
                _data.OnDestroy -= RemoveListener;
            }
        }

        

    }
}