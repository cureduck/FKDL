using System;

namespace Game
{
    public sealed class ChestSaveData : MapData
    {
        public Rank Rank;
        public Offer[] Offers;


        public override void Init()
        {
            base.Init();
            switch (Rank)
            {
                case Rank.Normal:
                    break;
                case Rank.Uncommon:
                    break;
                case Rank.Rare:
                    break;
                case Rank.Ultra:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}