using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public static class Nightmares
    {
        public static readonly HashSet<string> NightmareLib = new HashSet<string>()
        {
            "nightmare_1",
            "nightmare_2",
            "nightmare_3",
            "nightmare_4",
            "nightmare_5",
        };

        private static float GetNightmareBonus(string id)
        {
            return .1f;
        }

        public static float GetAllNightmareBonus(IEnumerable<string> ids)
        {
            return ids.Where(id => NightmareLib.Contains(id)).Sum(GetNightmareBonus);
        }


        public static void ApplyNightMare(string id, Map map)
        {
            foreach (var floor in map.Floors)
            {
                foreach (var sq in floor.Value.Squares)
                {
                    if (sq is EnemySaveData e)
                    {
                        ApplyNightmare(id, e);
                    }
                }
            }
        }

        private static void ApplyNightmare(string id, EnemySaveData mapData)
        {
            switch (id)
            {
                case "nightmare_1":
                    mapData.Status.MaxHp = (int)(mapData.Status.MaxHp * 1.1f);
                    mapData.Status.CurHp = mapData.Status.MaxHp;
                    break;
                case "nightmare_2":
                    mapData.Status.PAtk = (int)(mapData.Status.PAtk * 1.1f);
                    mapData.Status.MAtk = (int)(mapData.Status.MAtk * 1.1f);
                    break;
                case "nightmare_3":
                    mapData.Status.PDef = (int)(mapData.Status.PDef * 1.1f);
                    mapData.Status.MDef = (int)(mapData.Status.MDef * 1.1f);
                    break;
                case "nightmare_4":
                    foreach (var skill in mapData.Skills)
                    {
                        skill.CurLv += 1;
                    }

                    break;
                case "nightmare_5":
                    if (mapData.Bp.Rank >= Rank.Rare)
                    {
                        mapData.Status.MaxHp = (int)(mapData.Status.MaxHp * 1.1f);
                        mapData.Status.CurHp = mapData.Status.MaxHp;
                        mapData.Status.PAtk = (int)(mapData.Status.PAtk * 1.1f);
                        mapData.Status.MAtk = (int)(mapData.Status.MAtk * 1.1f);
                        mapData.Status.PDef = (int)(mapData.Status.PDef * 1.1f);
                        mapData.Status.MDef = (int)(mapData.Status.MDef * 1.1f);
                    }

                    break;
            }
        }
    }
}