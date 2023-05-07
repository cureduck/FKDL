using System;
using Managers;

namespace Game
{
    public class SupplySaveData : MapData
    {
        public SupplyType Type;
        public Rank Rank;

        public SupplySaveData(SupplyType supplyType) : base()
        {
            Type = supplyType;
            Rank = _rank;
        }

        private int value
        {
            get
            {
                switch (Rank)
                {
                    case Rank.Normal:
                        return 10;
                    case Rank.Uncommon:
                        return 20;
                    case Rank.Rare:
                        return 30;
                    case Rank.Ultra:
                        break;
                    default:
                        return 0;
                }
                return 0;
            }
        }
        
        public override void OnReact()
        {
            base.OnReact();

            switch (Type)
            {
                case SupplyType.Spring:
                    GameManager.Instance.PlayerData.Heal(new BattleStatus{CurMp = value}, "supply");
                    break;
                case SupplyType.Grassland:
                    GameManager.Instance.PlayerData.Heal(new BattleStatus{CurHp = value}, "supply");
                    break;
                default:
                    break;
            }
            
            Destroyed();
        }

        public override SquareInfo GetSquareInfo()
        {
            switch (Type)
            {
                case SupplyType.Spring:
                    return new SquareInfo()
                    {
                        Name = "spring",
                        Desc = "spring_desc",
                        P1 = value.ToString()
                    };
                case SupplyType.Grassland:
                    return new SquareInfo()
                    {
                        Name = "grassland",
                        Desc = "grassland_desc",
                        P1 = value.ToString()
                    };
                case SupplyType.Camp:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    public enum SupplyType
    {
        Spring,
        Grassland,
        Camp
    }
    
    
}