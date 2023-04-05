using System.Collections.Generic;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class Relic : CsvData
    {
        [JsonIgnore] public Texture Icon;

        public Relic(Rank rank, string id) : base(rank, id)
        {
            
        }

        public Relic() :base()
        {
            
        }
    }
}