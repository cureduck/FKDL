using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Csv;
using Game;
using I2.Loc;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Tools;
using UnityEngine;
using Random = System.Random;


namespace Managers
{
    [ExecuteAlways]
    public class RelicManager : XMLDataManager<Relic, RelicData>
    {
        protected override string CsvPath => Paths.RelicDataPath;

        protected override Relic Line2T(ICsvLine line)
        {
            bool usedUp = bool.TryParse(line["usedup"], out var uu) && uu;
            bool useCounter = bool.TryParse(line["usecounter"], out var us) && us;
            float param = float.TryParse(line["param"], out var up) ? up : 0f;

            return new Relic((Rank)int.Parse(line["rank"]), line["id"].ToLower(), param, usedUp, useCounter);
        }

        protected override Relic CreateTest(string id, MethodInfo method, EffectAttribute attr)
        {
            return new Relic(Rank.Normal, id, 1f, true, true);
        }

        protected override IEnumerable<Relic> GetCandidates(Rank rank)
        {
            var banList = GetBanList();
            var chosen = base.GetCandidates(rank).Where(relic => !banList.Contains(relic.Id)).ToArray();
            banList.AddRange(chosen.Select(relic => relic.Id));
            return chosen;
        }

        private List<string> GetBanList()
        {
            return GameDataManager.Instance.SecondaryData.DiscoveredRelics;
        }
    }
}