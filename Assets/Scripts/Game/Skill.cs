using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class Skill : CsvData
    {
        public readonly HashSet<Timing> AlwaysActiveTiming;
        public readonly bool CanGet;
        public bool BattleOnly;
        public int Cooldown;
        public CostInfo CostInfo;
        public int MaxLv;

        public float Param1;
        public float Param2;
        public bool Positive;
        [FormerlySerializedAs("Pool")] public string Prof;

        public Skill(Rank rank, string id, bool canGet, Sprite icon) : base(rank, id, icon)
        {
            CanGet = canGet;
            AlwaysActiveTiming = new HashSet<Timing>();
        }


        public Skill(Rank rank, string id) : base(rank, id)
        {
        }

        public Skill()
        {
        }
    }
}