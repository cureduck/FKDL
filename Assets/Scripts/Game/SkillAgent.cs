using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Tools;
using Random = UnityEngine.Random;

namespace Game
{
    public class SkillAgent : List<SkillData>
    {


        public int EmptySlot => this.Count((data => data == SkillData.Empty));
        
        public SkillAgent(SkillData[] bp)
        {
            if (bp.Length == 0)
            {
                return;
            }
            for (int i = 0; i < bp.Length; i++)
            {
                Add((SkillData)bp[i].Clone());
            }
        }

        public SkillAgent()
        {
            
        }

        public IEnumerable<SkillData> ActiveSkills()
        {
            return this.Where((data => (data != null && !data.IsEmpty && data.Bp != null)));
        }
        

        public void AddSkillSlot(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Add(SkillData.Empty);
            }
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


        public bool CooldownRandomSkill(int count = 1)
        {
            return false;
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