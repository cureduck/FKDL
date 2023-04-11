using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    public abstract class CellView<T> : SerializedMonoBehaviour
    {
        public T Data;
        
        public abstract void UpdateUI();


        /// <summary>
        /// 可以用来做消失动画
        /// </summary>
        public virtual void Removed()
        {
            if (GetComponent<SpriteRenderer>() != null)
            {
                GetComponent<SpriteRenderer>().DOFade(0f, 1f)
                    .OnComplete((() => Destroy(gameObject)));
            }
            
            Destroy(gameObject);
        }

    }
}