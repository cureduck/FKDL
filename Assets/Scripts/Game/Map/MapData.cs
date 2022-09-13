using System;
using Unity.Mathematics;
using UnityEngine;

namespace Game
{
    public class MapData
    {
        public int x;
        public int y;
        public int Width;
        public int Height;

        public virtual void Init(){}
    }
}