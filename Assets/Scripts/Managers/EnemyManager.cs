using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Managers
{
    [ExecuteAlways]
    public class EnemyManager : Singleton<EnemyManager>
    {
        public Dictionary<string, EnemyBp> EnemyBps;

        private void Start()
        {
            EnemyBps = new Dictionary<string, EnemyBp>();
            var bps = Resources.LoadAll<EnemyBp>("EnemyBp");
            foreach (var bp in bps)
            {
                EnemyBps[bp.Id.ToLower().Trim()] = bp;
                if (bp.Skills == null)
                {
                    bp.Skills = new SkillData[0];
                }

                if (bp.Buffs == null)
                {
                    bp.Buffs = new BuffAgent();
                }

                if (!SpriteManager.Instance.BuffIcons.TryGetValue(bp.Id.ToLower(), out bp.Icon))
                {
                    bp.Icon = SpriteManager.Instance.BuffIcons["soldier"];
                }
            }
        }


        public bool TryGetEnemy(string id, out EnemyBp bp)
        {
            return EnemyBps.TryGetValue(id.ToLower().Trim(), out bp);
        }
    }
}