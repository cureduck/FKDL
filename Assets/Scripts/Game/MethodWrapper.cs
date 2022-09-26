using System.Reflection;

namespace Game
{
    public struct MethodWrapper
    {
        public MethodInfo MethodInfo;
        public int Priority;
        public Timing Timing;
        
        public static implicit operator MethodWrapper(MethodInfo methodInfo)
        {
            var attr = methodInfo.GetCustomAttribute<SkillEffectAttribute>();
            return new MethodWrapper
            {
                MethodInfo = methodInfo,
                Priority = attr.priority,
                Timing = attr.timing
            };
        }
    }
}