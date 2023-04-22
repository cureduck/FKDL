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
            var t = Enum.TryParse<BuffType>(line["bufftype"], true, out var buffType);
            var tt = bool.TryParse(line["stackable"], out var stackable);
            var oppositeId = line["oppositeId"].ToLower();
            var id = line["id"].ToLower();
            var icon = GetIcon(id);
            var buff = new Buff(id, Rank.Normal, stackable, icon, buffType, oppositeId);
            return buff;
        }

        protected override Buff CreateTest(string id, MethodInfo method, EffectAttribute attr)
        {
            return new Buff(id, Rank.Normal, true, null, BuffType.Positive, null);
        }
    }
}