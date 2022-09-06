using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class Square<T> : MonoBehaviour where T: MapData
    {
        public SquareBase Sq;

        [ShowInInspector] public T Data;

        protected virtual void Start()
        {
            Sq = GetComponent<SquareBase>();
        }
    }
}