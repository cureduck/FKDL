using Managers;

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
            Player.AppliedBuff(new BuffData(BuffId, 1));
            base.OnReact();
            Destroyed();
        }
    }
}