using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Game
{
    public class MapData
    {
        public Placement Placement;

        public bool Revealed = false;
        
        /// <summary>
        /// 新游戏时调用
        /// </summary>
        public virtual void Init(){}
        
        /// <summary>
        /// 加载游戏时调用
        /// </summary>
        public virtual void Load(){}
        
        public virtual void OnFocus(){}
        
        public virtual void OnReact(){}
        
        public event Action OnDestroy;
        public event Action OnUpdated;

        protected void Destroyed()
        {
            RevealAround();
            OnDestroy?.Invoke();
        }

        protected void Updated()
        {
            OnUpdated?.Invoke();
        }

        [JsonIgnore] public int Area => Placement.Height * Placement.Width;

        [JsonIgnore]
        protected Rank _rank
        {
            get
            {
                if (Area <= 4)
                {
                    return Rank.Normal;
                }

                return Area>=16 ? Rank.Rare : Rank.Uncommon;
            }
        }

        public void RevealAround()
        {
            
        }
        
    }


    public struct Placement
    {
        public int x;
        public int y;
        public int Width;
        public int Height;
    }
    
}