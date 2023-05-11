using Managers;

namespace Game
{
    public class GuineasSaveData : MapData
    {
        public int Count;

        public GuineasSaveData(int count) : base()
        {
            Count = count;
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OnReact()
        {
            base.OnReact();
            GameManager.Instance.PlayerData.Gain(Count);
            Destroyed();
        }

        public override SquareInfo GetSquareInfo()
        {
            var msg = base.GetSquareInfo();
            msg.P1 = Count.ToString();
            return msg;
        }
    }
}