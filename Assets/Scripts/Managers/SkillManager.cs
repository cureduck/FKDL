using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Csv;
using Game;

namespace Managers
{
    public class SkillManager : XMLDataManager<Skill, SkillData, SkillManager>
    {
        protected override string CsvPath => Paths.SkillDataPath;

        protected override Skill Line2T(ICsvLine line)
        {
            int.TryParse(line["cd"], out var cooldown);
            int.TryParse(line["cost"], out var cost);
            var id = line["id"].ToLower();
            var icon = GetIcon(id);
            bool.TryParse(line["CanGet"], out var canGet);
            var keywords = line["Keywords"].Split(',');
            var effects = line["Effects"];
            return new Skill((Rank)int.Parse(line["Rarity"]), id, canGet, icon, keywords, effects)
            {
                Prof = line["Pool"],
                Positive = bool.Parse(line["Positive"]),
                BattleOnly = bool.Parse(line["BattleOnly"]),
                MaxLv = int.Parse(line["MaxLv"]),
                Param1 = float.Parse(line["P1"] != "" ? line["P1"] : "0"),
                Param2 = float.Parse(line["P2"] != "" ? line["P2"] : "0"),
                CostInfo = new CostInfo(cost, line["CostType"] == "" ? CostType.Mp : CostType.Hp),
                Cooldown = cooldown,
            };
        }


        protected override void Bind(Skill v, MethodInfo method, EffectAttribute attr)
        {
            if (attr.alwaysActive) v.AlwaysActiveTiming.Add(attr.timing);
            base.Bind(v, method, attr);
        }

        protected override IEnumerable<Skill> GetCandidates(Rank rank)
        {
            var candidates = base.GetCandidates(rank);
            return candidates.Where(skill => skill.CanGet && SData.Profs.Contains(skill.Prof.ToUpper()));
        }

        public Skill[] GetProfSkills(string prof)
        {
            return Lib.Values.Where(skill => skill.Prof == prof).ToArray();
        }
    }
}