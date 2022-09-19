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

        public override void OnReact()
        {
            base.OnReact();

            int value = 0;
            switch (Rank)
            {
                case Rank.Normal:
                    value = 10;
                    break;
                case Rank.Uncommon:
                    value = 20;
                    break;
                case Rank.Rare:
                    value = 30;
                    break;
                case Rank.Ultra:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (Type)
            {
                case SupplyType.Spring:
                    GameManager.Instance.PlayerData.Status.CurMp += value;
                    break;
                case SupplyType.Grassland:
                    GameManager.Instance.PlayerData.Status.CurHp += value;
                    break;
                case SupplyType.Camp:
                    GameManager.Instance.PlayerData.PlayerStatus.CurSp += value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Destroyed();
        }
    }
    public enum SupplyType
    {
        Spring,
        Grassland,
        Camp
    }
}