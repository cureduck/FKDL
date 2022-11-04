using System.Collections.Generic;

namespace Game
{
    public class SkillAgent : List<SkillData>, IEffectContainer
    {

        public SkillAgent(SkillData[] bp)
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

        public SkillAgent()
        {
            
        }
        
        
        public bool MayAffect(Timing timing, out int priority)
        {
            throw new System.NotImplementedException();
        }

        public T Affect<T>(Timing timing, object[] param)
        {
            throw new System.NotImplementedException();
        }
    }
}