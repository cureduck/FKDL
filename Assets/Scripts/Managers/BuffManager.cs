using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Csv;
using Game;
using UI;
using UnityEngine;

namespace Managers
{
    public class BuffManager : XMLDataManager<Buff, BuffData>
    {
        protected override string CsvPath => Paths.BuffDataPath;

        protected override Buff Line2T(ICsvLine line)
        {
            bool positive = line["positive"].ToUpper() == "TRUE";
            string oppositeId = line["oppositeId"].ToLower();
            var id = line["id"].ToLower();
            var icon = GetIcon(id);
            var buff = new Buff(id, Rank.Normal, icon, positive, oppositeId);
            return buff;
        }
    }
}