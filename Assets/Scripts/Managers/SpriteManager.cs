using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class SpriteManager : Singleton<SpriteManager>
    {
        private const string IconPath = "BuffIcon";
        public Dictionary<string, Sprite> BuffIcons;
        
        
        
        protected override void Awake()
        {
            base.Awake();
            BuffIcons = new Dictionary<string, Sprite>();
            foreach (var sprite in Resources.LoadAll<Sprite>(IconPath))
            {
                BuffIcons.Add(sprite.name, sprite);
            }
        }
    }
}