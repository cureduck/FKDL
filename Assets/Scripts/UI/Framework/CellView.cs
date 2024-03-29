﻿using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

namespace UI
{
    public interface IActivated
    {
        event Action Activated;
    }

    public interface ISimpleDataHolder
    {
        string Id { get; }
        string Desc { get; }
        string Param { get; }
    }


    [DisallowMultipleComponent]
    public abstract class CellView<T> : MonoBehaviour, ISimpleDataHolder
    {
        public T Data;

        public abstract void UpdateUI();

        public void Bind()
        {
            if (Data is IActivated d)
            {
                d.Activated += OnActivated;
            }
        }

        public void UnBind()
        {
            if (Data is IActivated d)
            {
                d.Activated -= OnActivated;
            }
        }

        /// <summary>
        /// 效果触发时播放的动画
        /// </summary>
        protected virtual void OnActivated()
        {
        }


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

        public abstract string Id { get; }
        public abstract string Desc { get; }
        public abstract string Param { get; }
    }
}