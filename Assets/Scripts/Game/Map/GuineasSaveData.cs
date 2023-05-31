using Unity.Plastic.Newtonsoft.Json;

namespace Game
{
    public class GuineasSaveData : MapData
    {
        [JsonProperty] private readonly int Level;
        public int Value;

        public GuineasSaveData(int level) : base()
        {
            Level = level;
        }

        public override void Init()
        {
            base.Init();
            Value = SData.NextInt(2 * Level + 1, 3 * Level + 1);
        }

        public override void OnReact()
        {
            base.OnReact();
            Player.Gain(Value);
            Destroyed();
        }

        public override SquareInfo GetSquareInfo()
        {
            var msg = base.GetSquareInfo();
            msg.P1 = Value.ToString();
            return msg;
        }
    }
}