using System.Linq;
using Managers;
using Sirenix.Utilities;

namespace Game
{
    public class TotemSaveData : MapData
    {
        public string BuffId;

        public override void Init()
        {
            base.Init();
            BuffId = ((BuffManager)BuffManager.Instance).GetRandomBuff(BuffType.Blessing);
        }


        public override void OnReact()
        {
            var bless = Player.Buffs.FirstOrDefault(da => da.Bp.BuffType == BuffType.Blessing);
            if (bless != null && !bless.Id.IsNullOrWhitespace())
            {
                Player.RemoveBuff(bless);
            }

            var curse = Player.Buffs.FirstOrDefault(da => da.Bp.BuffType == BuffType.Curse);
            if (curse != null && !curse.Id.IsNullOrWhitespace())
            {
                Player.RemoveBuff(curse);
            }
            else
            {
                Player.AppliedBuff(new BuffData(BuffId, 1));
            }

            base.OnReact();
            Destroyed();
        }
    }
}