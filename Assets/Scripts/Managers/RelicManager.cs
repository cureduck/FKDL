using System.Collections.Generic;
using System.Linq;
using Csv;
using Game;
using UnityEngine;

namespace Managers
{
    [ExecuteAlways]
    public class RelicManager : XMLDataManager<Relic, RelicData>
    {
        protected override string CsvPath => Paths.RelicDataPath;

        protected override Relic Line2T(ICsvLine line)
        {
            var icon = GetIcon(line["id"].ToLower());
            bool usedUp = bool.TryParse(line["usedup"], out var uu) && uu;
            bool useCounter = bool.TryParse(line["usecounter"], out var us) && us;
            float param = float.TryParse(line["param"], out var up) ? up : 0f;

            return new Relic((Rank)int.Parse(line["rank"]), line["id"].ToLower(), param, usedUp, useCounter, icon);
        }

        public override Relic[] RollT(Rank rank, int count = 1)
        {
            var chosen = base.RollT(rank, count);
            GetBanList().AddRange(chosen.Select(relic => relic.Id));
            return chosen;
        }


        protected override IEnumerable<Relic> GetCandidates(Rank rank)
        {
            var banList = GetBanList();
            var chosen = base.GetCandidates(rank).Where(relic => !banList.Contains(relic.Id)).ToArray();
            return chosen;
        }

        private List<string> GetBanList()
        {
            return GameDataManager.Instance.SecondaryData.DiscoveredRelics;
        }
    }
}