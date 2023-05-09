using System;
using System.Linq;
using Csv;
using Game;
using Tools;

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

        public string GetRandomBuff(BuffType buffType)
        {
            return Lib.Values.Where((buff => buff.BuffType == buffType)).ChooseRandom(Lib.Random).Id;
        }
    }
}