using System.Collections.Generic;
using System.Reflection;
using Csv;
using Game;
using Tools;

namespace Managers
{
    interface IEffector
    {
        void AddEffect(Timing timing, MethodInfo methodInfo);
        Dictionary<Timing, MethodInfo> Fs { get; }
    }
    
    public abstract class XMLDataManager <T> : Singleton<XMLDataManager<T>> where T : IRank
    {
        private CustomDictionary<T> Lib;

        private void Load()
        {
            Lib = new CustomDictionary<T>();
        }


        protected abstract T Line2T(ICsvLine line);

    }
}