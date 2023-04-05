using System.Collections.Generic;
using System.Reflection;
using Game;
using Sirenix.OdinInspector;

namespace Managers
{

    public class CsvData : IRank
    {
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

        public void AddEffect(Timing timing, MethodInfo methodInfo)
        {
            Fs[timing] = methodInfo;
        }

        public Dictionary<Timing, MethodInfo> Fs { get; }


        public string Id { get; set; }
        public Rank Rank { get; }
    }
}