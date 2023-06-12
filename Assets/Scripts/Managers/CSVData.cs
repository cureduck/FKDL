using System.Collections.Generic;
using System.Reflection;
using Game;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    public class
        CsvData : IRank
    {
        protected CsvData(Rank rank, string id, Sprite icon) : this(rank, id)
        {
            Icon = icon;
        }

        protected CsvData(Rank rank, string id) : this(rank)
        {
            Id = id;
        }

        protected CsvData()
        {
            Fs = new Dictionary<Timing, MethodInfo>();
        }

        protected CsvData(Rank rank) : this()
        {
            Rank = rank;
        }

        [JsonIgnore, ShowInInspector] public Sprite Icon { get; }

        [ShowInInspector, JsonIgnore] public Dictionary<Timing, MethodInfo> Fs { get; }


        public string Id { get; set; }
        public Rank Rank { get; }

        public void AddEffect(Timing timing, MethodInfo methodInfo)
        {
            Fs[timing] = methodInfo;
        }
    }
}