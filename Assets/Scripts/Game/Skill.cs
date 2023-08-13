using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class Skill : CsvData, IKeyword
    {
        public readonly HashSet<Timing> AlwaysActiveTiming;
        public readonly bool CanGet;
        public readonly string SE;
        public bool BattleOnly;
        public int Cooldown;
        public CostInfo CostInfo;
        public int MaxLv;

        public float Param1;
        public float Param2;
        public bool Positive;
        [FormerlySerializedAs("Pool")] public string Prof;

        public Skill(Rank rank, string id, bool canGet, Sprite icon, string[] keywords = null,
            string se = null) : base(rank, id, icon)
        {
            CanGet = canGet;
            Keywords = keywords;
            AlwaysActiveTiming = new HashSet<Timing>();
            SE = se;
        }


        public Skill(Rank rank, string id) : base(rank, id)
        {
        }

        public Skill()
        {
        }

        public string[] Keywords { get; }
    }
}