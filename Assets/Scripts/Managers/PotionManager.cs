﻿using System.Linq;
using Csv;
using Game;

namespace Managers
{
    public class PotionManager : XMLDataManager<Potion, PotionData, PotionManager>
    {
        private static string[] AttrPotionIds =
            { "hppotion++", "mppotion++", "patkpotion", "matkpotion", "pdefpotion", "mdefpotion" };

        protected override string CsvPath => Paths.PotionDataPath;

        protected override Potion Line2T(ICsvLine line)
        {
            var id = line["id"].ToLower().Replace(" ", "");
            var rank = (Rank)int.Parse(line["rarity"]);
            var icon = GetIcon(id);
            if (icon == null) icon = GetIcon(id.Replace("+", ""));
            var upgrade = line["upgrade"];
            var keywords = line["keywords"].Split(',').Select(s => s.Trim()).ToArray();

            return new Potion(rank, id, upgrade, keywords, icon)
            {
                Param1 = float.Parse(line["P1"]),
                Param2 = float.Parse(line["P2"]),
            };
        }

        public Potion[] GetAttrPotion(int count)
        {
            var chosen = AttrPotionIds.OrderBy((s => SData.CurGameRandom.NextDouble())).Take(count);

            return chosen.Select(GetById).ToArray();
        }
    }
}