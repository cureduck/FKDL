using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Managers
{
    [ExecuteAlways]
    public class SpriteManager : Singleton<SpriteManager>
    {
        public enum IconType
        {
            ChooseProf,
            Prof,
            Buff,
            Relic,
        }

        private const string IconPath = "SourceImages";
        public Dictionary<string, Sprite> BuffIcons;

        private Dictionary<IconType, Dictionary<string, Sprite>> iconDatas;

        public Dictionary<Rank, Sprite> PotionBottleIcon;

        protected override void Awake()
        {
            base.Awake();
            //DontDestroyOnLoad(gameObject);
            BuffIcons = new Dictionary<string, Sprite>();
            foreach (var sprite in Resources.LoadAll<Sprite>(IconPath))
            {
                if (!BuffIcons.ContainsKey(sprite.name))
                {
                    //Debug.Log(sprite);
                    BuffIcons[sprite.name.ToLower()] = sprite;
                }
            }

            iconDatas = new Dictionary<IconType, Dictionary<string, Sprite>>();
            //职业图标
            Dictionary<string, Sprite> ProfIcons = new Dictionary<string, Sprite>();
            Sprite[] icons = Resources.LoadAll<Sprite>("SourceImages/Icon/Profs");
            for (int i = 0; i < icons.Length; i++)
            {
                ProfIcons.Add(icons[i].name, icons[i]);
            }

            iconDatas.Add(IconType.Prof, ProfIcons);

            //状态图标
            Dictionary<string, Sprite> newBuffIcons = new Dictionary<string, Sprite>();
            icons = Resources.LoadAll<Sprite>("SourceImages/Icon/Buff");
            for (int i = 0; i < icons.Length; i++)
            {
                newBuffIcons.Add(icons[i].name, icons[i]);
            }

            iconDatas.Add(IconType.Buff, newBuffIcons);

            //魔器图标
            Dictionary<string, Sprite> relicIcons = new Dictionary<string, Sprite>();
            icons = Resources.LoadAll<Sprite>("SourceImages/Icon/Relic");
            for (int i = 0; i < icons.Length; i++)
            {
                relicIcons.Add(icons[i].name, icons[i]);
            }

            iconDatas.Add(IconType.Relic, relicIcons);

            //职业选择详情图标
            Dictionary<string, Sprite> chooseProfIcons = new Dictionary<string, Sprite>();
            icons = Resources.LoadAll<Sprite>("SourceImages/Icon/ChooseProf");
            for (int i = 0; i < icons.Length; i++)
            {
                chooseProfIcons.Add(icons[i].name, icons[i]);
            }

            iconDatas.Add(IconType.ChooseProf, chooseProfIcons);
        }

        public Sprite GetIcon(IconType iconType, string stringID)
        {
            Dictionary<string, Sprite> cur;

            if (iconDatas.TryGetValue(iconType, out cur))
            {
                Sprite curIcon;
                cur.TryGetValue($"{stringID}", out curIcon);
                return curIcon;
            }
            else
            {
                return null;
            }
        }
    }
}