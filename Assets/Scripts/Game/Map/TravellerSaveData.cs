using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Game
{
    public class TravellerSaveData : MapData
    {
        [JsonProperty, ShowInInspector] private readonly string Line;

        [JsonConstructor]
        public TravellerSaveData(string line)
        {
            Line = line;
        }

        public override SquareInfo GetSquareInfo()
        {
            return new SquareInfo
            {
                Name = "traveller",
                Desc = Line.Replace("*", "1"),
            };
        }


        public override void OnReact()
        {
            Player.Gain(1);
            base.OnReact();
            Destroyed();
        }
    }
}