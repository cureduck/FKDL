﻿using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Managers
{
    [ExecuteAlways]
    public class SpriteManager : Singleton<SpriteManager>
    {
        private const string IconPath = "SourceImages";
        public Dictionary<string, Sprite> BuffIcons;

        public Dictionary<Rank, Sprite> PotionBottleIcon;
        
        protected override void Awake()
        {
            base.Awake();
            BuffIcons = new Dictionary<string, Sprite>();
            foreach (var sprite in Resources.LoadAll<Sprite>(IconPath))
            {
                if (!BuffIcons.ContainsKey(sprite.name))
                {
                    //Debug.Log(sprite);
                    BuffIcons[sprite.name.ToLower()] = sprite;
                }
            }
        }
    }
}