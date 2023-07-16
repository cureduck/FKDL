using Managers;
using UnityEngine;

namespace Game
{
    public class Relic : CsvData
    {
        public string[] Keywords;
        public float Param1;
        public bool UseCounter;
        public bool UsedUp;

        public Relic(Rank rank, string id, float param, bool usedUp, bool useCounter, string[] keywords,
            Sprite icon) : base(rank, id,
            icon)
        {
            Param1 = param;
            UsedUp = usedUp;
            UseCounter = useCounter;
            Keywords = keywords;
        }
    }
}