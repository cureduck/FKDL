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

        [JsonIgnore] public Action OnDestroy;
        [JsonIgnore] public Action OnUpdated;
    }


    public struct Placement
    {
        public int x;
        public int y;
        public int Width;
        public int Height;
    }
}