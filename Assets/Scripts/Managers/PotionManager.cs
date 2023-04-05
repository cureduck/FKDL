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
            return new Potion
            {
                Id = line["id"].ToLower().Replace(" ", ""),
                Rank = (Rank) int.Parse(line["rarity"]),
                Param1 = float.Parse(line["P1"]),
            };
        }
    }
}