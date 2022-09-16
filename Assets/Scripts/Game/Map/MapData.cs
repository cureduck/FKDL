using System;
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
    }


    public struct Placement
    {
        public int x;
        public int y;
        public int Width;
        public int Height;
    }
}