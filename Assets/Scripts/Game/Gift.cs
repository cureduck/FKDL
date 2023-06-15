using System.Collections.Generic;
using Game.PlayerCommands;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public abstract class Gift
    {
        private Dictionary<int, int> _levelCost = new Dictionary<int, int>()
        {
            { 0, 10 },
            { 1, 10 },
            { 2, 20 },
            { 3, 30 },
            { 4, 40 },
            { 5, 50 },
            { 6, 60 },
            { 7, 70 },
            { 8, 80 },
            { 9, 90 },
            { 10, 100 },
        };

        public int CurrentLevel { get; protected set; } = 1;
        [JsonIgnore] public virtual int LevelUpCost => _levelCost[CurrentLevel];
        public virtual int MaxLevel => 3;
        public abstract int PointCost { get; }

        public abstract PlayerCommand[] Commands { get; }

        public void Upgrade()
        {
            CurrentLevel++;
        }
    }


    public class RarePotionGift : Gift
    {
        public override int PointCost => 2;

        public override PlayerCommand[] Commands => CreateCommands();

        private PlayerCommand[] CreateCommands()
        {
            return new PlayerCommand[]
            {
                new GetPotionCommand(Mode.Random, null, CurrentLevel, Rank.Rare)
            };
        }
    }

    public class SkillSlotGift : Gift
    {
        public override int MaxLevel => 3;
        public override int PointCost => 6 - CurrentLevel;

        public override PlayerCommand[] Commands => CreateCommands();

        private PlayerCommand[] CreateCommands()
        {
            return new PlayerCommand[]
            {
                new GetSlotCommand(Mode.Random)
            };
        }
    }

    public class SkillPointGift : Gift
    {
        public override int MaxLevel => 2;
        public override int PointCost => 3;
        public override PlayerCommand[] Commands => CreateCommands();

        private PlayerCommand[] CreateCommands()
        {
            return new PlayerCommand[]
            {
                new SkillPointCommand(count: CurrentLevel)
            };
        }
    }

    public class RecoverPotionGift : Gift
    {
        public override int PointCost => 2;
        public override PlayerCommand[] Commands => CreateCommands();

        private PlayerCommand[] CreateCommands()
        {
            return new PlayerCommand[]
            {
                new GetPotionCommand(Mode.Random, null, CurrentLevel * 2, Rank.Normal),
                new GetPotionCommand(Mode.Random, null, CurrentLevel * 2, Rank.Normal),
                new GetPotionCommand(Mode.Random, null, CurrentLevel * 2, Rank.Normal)
            };
        }
    }

    public class RandomSkillGift : Gift
    {
        public override int MaxLevel => 2;
        public override int PointCost => 3;
        public override PlayerCommand[] Commands => CreateCommands();

        private PlayerCommand[] CreateCommands()
        {
            return new PlayerCommand[]
            {
                new GetSkillCommand(Mode.Random, null, CurrentLevel, Rank.Rare)
            };
        }
    }

    public abstract class AttrGift : Gift
    {
        public override int PointCost => 1;
        public override PlayerCommand[] Commands => CreateCommands();

        protected abstract string AttrName { get; }
        protected abstract int GiftCount { get; }

        protected PlayerCommand[] CreateCommands()
        {
            return new PlayerCommand[]
            {
                new GetAttrCommand(AttrName, GiftCount)
            };
        }
    }

    public class MaxHpGift : AttrGift
    {
        protected override string AttrName => "maxhp";
        protected override int GiftCount => CurrentLevel * 10 + 10;
    }

    public class MaxMpGift : AttrGift
    {
        protected override string AttrName => "maxmp";
        protected override int GiftCount => CurrentLevel * 10 + 10;
    }

    public class PAtkGift : AttrGift
    {
        protected override string AttrName => "patk";
        protected override int GiftCount => CurrentLevel;
    }

    public class MAtkGift : AttrGift
    {
        protected override string AttrName => "matk";
        protected override int GiftCount => CurrentLevel;
    }


    public class Gifts : SaveData
    {
        public const int MaxPoint = 5;

        public static readonly Dictionary<string, Gift> GiftDictionary = new Dictionary<string, Gift>
        {
            { "Gift_RarePotion", new RarePotionGift() },
            { "Gift_SkillSlot", new SkillSlotGift() },
            { "Gift_SkillPoint", new SkillPointGift() },
            { "Gift_RecoverPotion", new RecoverPotionGift() },
            { "Gift_RandomSkill", new RandomSkillGift() },
            { "Gift_MaxHp", new MaxHpGift() },
            { "Gift_MaxMp", new MaxMpGift() },
            { "Gift_PAtk", new PAtkGift() },
            { "Gift_MAtk", new MAtkGift() }
        };

        public List<Gift> SelectedGifts;
        public int SpentPoint = 0;

        public Gifts()
        {
            SelectedGifts = new List<Gift>();
        }

        private string _path => Application.streamingAssetsPath + "/Gifts.asset";

        public void SelectGift(Gift gift)
        {
            SpentPoint += gift.PointCost;
            SelectedGifts.Add(gift);
        }

        private static Gifts CreateDefault()
        {
            return new Gifts();
        }

        public static Gifts GetOrCreate()
        {
            return GetOrCreate(CreateDefault, Application.streamingAssetsPath + "/Gifts.asset");
        }

        public void Save()
        {
            Save(_path);
        }
    }
}