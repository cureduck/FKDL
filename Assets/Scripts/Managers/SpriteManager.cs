using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Managers
{
    public class SpriteManager : Singleton<SpriteManager>
    {
        private const string IconPath = "BuffIcon";
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
                    BuffIcons.Add(sprite.name, sprite);
                }
            }
        }
    }
}