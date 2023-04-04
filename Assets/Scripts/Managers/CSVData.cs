using System.Collections.Generic;
using System.Reflection;
using Game;

namespace Managers
{
    public class CsvData : IEffector, IRank
    {
        public CsvData(Dictionary<Timing, MethodInfo> fs)
        {
            Fs = fs;
        }

        public void AddEffect(Timing timing, MethodInfo methodInfo)
        {
            Fs[timing] = methodInfo;
        }

        public Dictionary<Timing, MethodInfo> Fs { get; private set; }

        public Rank Rank { get; }
    }
}