using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Csv;
using Game;

namespace Managers
{
    public class SkillManager : XMLDataManager<Skill, SkillData>
    {
        protected override string CsvPath => Paths.SkillDataPath;

        protected override Skill Line2T(ICsvLine line)
        {
            int.TryParse(line["cd"], out var cooldown);
            int.TryParse(line["cost"], out var cost);
            var id = line["id"].ToLower();
            var icon = GetIcon(id);
            return new Skill((Rank)int.Parse(line["Rarity"]), id, icon)
            {
                Pool = line["Pool"],
                Positive = bool.Parse(line["Positive"]),
                BattleOnly = bool.Parse(line["BattleOnly"]),
                MaxLv = int.Parse(line["MaxLv"]),
                Param1 = float.Parse(line["P1"] != "" ? line["P1"] : "0"),
                Param2 = float.Parse(line["P2"] != "" ? line["P2"] : "0"),
                CostInfo = new CostInfo(cost, line["CostType"] == "" ? CostType.Mp : CostType.Hp),
                Cooldown = cooldown,
                //Description = line[10]
            };
        }


        protected override void Bind(Skill v, MethodInfo method, EffectAttribute attr)
        {
            if (attr.alwaysActive) v.AlwaysActiveTiming.Add(attr.timing);
            base.Bind(v, method, attr);
        }

        protected override IEnumerable<Skill> GetCandidates(Rank rank)
        {
            return base.GetCandidates(rank)
                .Where((skill => GameDataManager.Instance.SecondaryData.Prof.Contains(skill.Pool)));
        }
    }
}