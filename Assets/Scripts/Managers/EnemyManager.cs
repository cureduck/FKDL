using System;
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
                EnemyBps[bp.Id.ToLower()] = bp;
            }
        }
    }
}