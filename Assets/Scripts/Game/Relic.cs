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
        public float Param1;

        public Relic(Rank rank, string id, float param, bool usedUp, bool useCounter) : base(rank, id)
        {
            Param1 = param;
            UsedUp = usedUp;
            UseCounter = useCounter;
        }
    }
}