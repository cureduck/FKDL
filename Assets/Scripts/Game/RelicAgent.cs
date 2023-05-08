using System.Collections.Generic;
using Newtonsoft.Json;

namespace Game
{
    public class RelicAgent : List<RelicData>
    {
        [JsonConstructor]
        public RelicAgent(RelicData[] bp)
        {
            if (bp.Length == 0)
            {
                return;
            }

            for (int i = 0; i < bp.Length; i++)
            {
                Add(bp[i]);
            }
        }

        public RelicAgent()
        {
        }
    }
}