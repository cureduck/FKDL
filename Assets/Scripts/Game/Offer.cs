﻿namespace Game
{
    public struct Offer
    {
        public enum OfferKind
        {
            Potion,
            Skill,
            Gold,
            Relic,
            Key
        }


        public Offer(Skill skill, int cost = 0)
        {
            Kind = OfferKind.Skill;
            Rank = skill.Rank;
            Id = skill.Id;
            Cost = new CostInfo(cost);
            isSold = false;
        }


        public Offer(Potion potion, int cost = 0)
        {
            Kind = OfferKind.Potion;
            Rank = potion.Rank;
            Id = potion.Id;
            Cost = new CostInfo(cost);
            isSold = false;
        }

        public Offer(Relic relic, int cost = 0)
        {
            Kind = OfferKind.Relic;
            Rank = relic.Rank;
            Id = relic.Id;
            Cost = new CostInfo(cost);
            isSold = false;
        }
        
        
        /// <summary>
        /// 只用来生成钥匙奖励
        /// </summary>
        /// <param name="keyRank"></param>
        /// <param name="cost"></param>
        public Offer(Rank keyRank, int cost = 0)
        {
            Kind = OfferKind.Key;
            Rank = keyRank;
            Id = null;
            Cost = new CostInfo(cost);
            isSold = false;
        }

        public bool isSold;//用于标记商品是否被出售
        public OfferKind Kind;
        public Rank Rank;
        public string Id;

        /// <summary>
        /// 花费的金币，若OfferKind = Gold，则将其设为正值即可变成获得金币
        /// </summary>
        public CostInfo Cost;
    }
}