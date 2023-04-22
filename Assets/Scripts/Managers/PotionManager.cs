using System;
using System.Collections.Generic;
using Csv;
using Game;
using Sirenix.OdinInspector;
using System.IO;
using System.Linq;
using System.Reflection;
using Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class PotionManager : XMLDataManager<Potion, PotionData>
    {
        protected override string CsvPath => Paths.PotionDataPath;

        protected override Potion Line2T(ICsvLine line)
        {
            var id = line["id"].ToLower().Replace(" ", "");
            var rank = (Rank) int.Parse(line["rarity"]);
            var icon = GetIcon(id);
            return new Potion(rank, id, icon)
            {
                Param1 = float.Parse(line["P1"]),
            };
        }

        protected override Potion CreateTest(string id, MethodInfo method, EffectAttribute attr)
        {
            throw new NotImplementedException();
        }
    }
}