using System.Collections.Generic;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class Relic : CsvData
    {
        public bool UsedUp;
        public bool UseCounter;

        public Relic(Rank rank, string id, bool usedUp, bool useCounter) : base(rank, id)
        {
            UsedUp = usedUp;
            UseCounter = useCounter;
        }
    }
}