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

        public virtual void Init(){}
        
        public virtual void OnFocus(){}
        
        public virtual void OnReact(){}
        
        public event Action OnDestroy;
        public event Action OnUpdated;

        protected void Destroy()
        {
            OnDestroy?.Invoke();
        }

        protected void Update()
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
    }


    public struct Placement
    {
        public int x;
        public int y;
        public int Width;
        public int Height;
    }
    
}