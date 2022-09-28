using System.Collections.Generic;
using System.Reflection;

namespace Game
{
    public interface IEffectContainer
    {
        bool MayAffect(Timing timing, out int priority);
        T Affect<T>(Timing timing, object[] param);
    }
}