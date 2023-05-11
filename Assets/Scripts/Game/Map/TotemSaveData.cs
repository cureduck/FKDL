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
            BuffId = ((BuffManager)BuffManager.Instance).GetRandomBuff(BuffType.Bless);
        }

        public override void OnReact()
        {
            var buff = Player.Buffs.FirstOrDefault(da => da.Bp.BuffType == BuffType.Bless);
            if (buff != null && !buff.Id.IsNullOrWhitespace())
            {
                Player.RemoveBuff(buff);
            }

            Player.AppliedBuff(new BuffData(BuffId, 1));
            base.OnReact();
            Destroyed();
        }
    }
}