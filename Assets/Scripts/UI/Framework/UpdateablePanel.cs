using System;
using UnityEngine;

namespace UI
{
    public abstract class UpdateablePanel<T> : BasePanel<T> where T : IUpdateable
    {
        protected override void SetData(T d)
        {
            if (Data != null)
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
            if (Data != null) 
            {
                Data.OnUpdated += UpdateUI;
                Data.OnDestroy += RemoveListener;
            }
        }

        private void RemoveListener()
        {
            if (Data != null)
            {
                Data.OnUpdated -= UpdateUI;
                Data.OnDestroy -= RemoveListener;
            }
        }

        

    }
}