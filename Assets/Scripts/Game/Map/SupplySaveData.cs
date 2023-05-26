using System;
using Managers;
using Newtonsoft.Json;

namespace Game
{
    public class SupplySaveData : MapData
    {
        public SupplyType Type;

        public SupplySaveData(SupplyType supplyType) : base()
        {
            Type = supplyType;
        }

        [JsonIgnore] public Rank Rank => _rank;

        private int Value
        {
            get
            {
                switch (Rank)
                {
                    case Rank.Normal:
                        return (int)(Player.Status.MaxHp * 0.1f);
                    case Rank.Uncommon:
                        return (int)(Player.Status.MaxHp * 0.2f);
                    case Rank.Rare:
                        return (int)(Player.Status.MaxHp * 0.3f);
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
                    GameManager.Instance.PlayerData.Heal(new BattleStatus { CurMp = Value }, "supply");
                    break;
                case SupplyType.Grassland:
                    GameManager.Instance.PlayerData.Heal(new BattleStatus { CurHp = Value }, "supply");
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
                        P1 = Value.ToString()
                    };
                case SupplyType.Grassland:
                    return new SquareInfo()
                    {
                        Name = "grassland",
                        Desc = "grassland_desc",
                        P1 = Value.ToString()
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