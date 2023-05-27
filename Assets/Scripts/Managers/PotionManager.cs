using Csv;
using Game;

namespace Managers
{
    public class PotionManager : XMLDataManager<Potion, PotionData, PotionManager>
    {
        protected override string CsvPath => Paths.PotionDataPath;

        protected override Potion Line2T(ICsvLine line)
        {
            var id = line["id"].ToLower().Replace(" ", "");
            var rank = (Rank)int.Parse(line["rarity"]);
            var icon = GetIcon(id);
            if (icon == null) icon = GetIcon(id.Replace("+", ""));
            var upgrade = line["upgrade"];

            return new Potion(rank, id, upgrade, icon)
            {
                Param1 = float.Parse(line["P1"]),
                Param2 = float.Parse(line["P2"]),
            };
        }
    }
}