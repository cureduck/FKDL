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
            var id = line[0].ToLower();
            var icon = GetIcon(id);
            var buff = new Buff (id, Rank.Normal, icon);
            return buff;
        }
    }
}