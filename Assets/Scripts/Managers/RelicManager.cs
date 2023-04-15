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

            
            return new Relic((Rank) int.Parse(line["rank"]),line["id"].ToLower(), usedUp, useCounter);
        }

        protected override IEnumerable<Relic> GetCandidates(Rank rank)
        {
            var banList = GetBanList();
            return base.GetCandidates(rank).Where(relic => !banList.Contains(relic.Id));
        }

        private IEnumerable<string> GetBanList()
        {
            throw new NotImplementedException();
        }
    }
}