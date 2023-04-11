using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    public abstract class CellView<T> : SerializedMonoBehaviour
    {
        public T Data;


        public abstract void UpdateUI();
        
    }
}