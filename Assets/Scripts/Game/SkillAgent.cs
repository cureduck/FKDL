using System.Collections.Generic;
using UnityEngine;

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

        public bool UpgradeRandomSkill(Rank rank)
        {
            var tmp = new List<SkillData>();
            foreach (var sk in this)
            {
                if ((sk.Bp.Rank == rank)&&(sk.CurLv < sk.Bp.MaxLv))
                {
                    tmp.Add(sk);
                }
            }

            if (tmp.Count == 0)
            {
                return false;
            }
            else
            {
                tmp[Random.Range(0, tmp.Count)].CurLv += 1;
                return true;
            }
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