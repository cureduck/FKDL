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
            return new Relic((Rank) int.Parse(line["rank"]),line["id"].ToLower());
        }
        
    }
}