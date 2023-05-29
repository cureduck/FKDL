#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using Game;
using OfficeOpenXml;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Managers
{
    public class MapLoader : Singleton<MapLoader>
    {
        [Button]
        private static void Load(string fileName)
        {
            DeletePrevious();
            var path = $"Assets/Maps/{fileName}.xlsx";

            var fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excel = new ExcelPackage(fs))
            {
                Debug.Log(excel.Workbook.Worksheets.Count);

                foreach (var sheet in excel.Workbook.Worksheets)
                {
                    var f = LoadFloor(sheet);

                    FloorManager.CreateFloor(f);
                }

                excel.Dispose();
                fs.Dispose();
            }
        }

        private static void DeletePrevious()
        {
            var path = Application.persistentDataPath + "/Floors";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);
        }

        [MenuItem("Editors/Create Map")]
        public static void CreateMap()
        {
            var max = 0;
            foreach (var file in Directory.GetFiles("Assets/Maps"))
            {
                if (file.EndsWith(".xlsx") && !file.Contains("~"))
                {
                    var t = file.Replace("test", "");
                    if (int.Parse(t) > max)
                    {
                        max = int.Parse(t);
                    }
                }
            }

            DeletePrevious();

            Load($"Assets/Maps/test{max}.xlsx");
        }


        private static Map.Floor LoadFloor(ExcelWorksheet worksheet)
        {
            var floor = new Map.Floor
            {
                FloorName = worksheet.Name.Split('_')[0],
                Squares = new LinkedList<MapData>()
            };

            foreach (var cell in worksheet.MergedCells)
            {
                var range = worksheet.Cells[cell];
                var p = new Placement
                {
                    Height = range.Rows,
                    Width = range.Columns,
                    y = range.Start.Row,
                    x = range.Start.Column
                };

                var s = worksheet.Cells[range.Start.Row, range.Start.Column].Value.ToString();
                var ss = s.Split('|');

                var prefix = ss[0];
                var suffix = ss.Length > 1 ? ss[1] : "";

                if (int.TryParse(suffix, out var v)) ;

                switch (prefix)
                {
                    case "treasure":
                        floor.Squares.AddLast(new ChestSaveData((Rank)v) { Placement = p });
                        break;
                    case "casino":
                        floor.Squares.AddLast(new CasinoSaveData() { Placement = p });
                        break;
                    case "grassland":
                        floor.Squares.AddLast(new SupplySaveData(SupplyType.Grassland) { Placement = p });
                        break;
                    case "spring":
                        floor.Squares.AddLast(new SupplySaveData(SupplyType.Spring) { Placement = p });
                        break;
                    case "camp":
                        floor.Squares.AddLast(new SupplySaveData(SupplyType.Camp) { Placement = p });
                        break;
                    case "shop":
                        floor.Squares.AddLast(new ShopSaveData() { Placement = p });
                        break;
                    case "door":
                        floor.Squares.AddLast(new DoorSaveData((Rank)v) { Placement = p });
                        break;
                    case "key":
                        floor.Squares.AddLast(new KeySaveData((Rank)v) { Placement = p });
                        break;
                    case "crystal":
                        floor.Squares.AddLast(new CrystalSaveData((Rank)v) { Placement = p });
                        break;
                    case "traveler":
                        floor.Squares.AddLast(new TravellerSaveData() { Placement = p });
                        break;
                    case "enemy":
                        floor.Squares.AddLast(new EnemySaveData(suffix.ToLower()) { Placement = p });
                        EnemyLegalCheck(suffix.ToLower());
                        break;
                    case "rock":
                        floor.Squares.AddLast(new RockSaveData() { Placement = p });
                        break;
                    case "obsidian":
                        floor.Squares.AddLast(new ObsidianSaveData() { Placement = p });
                        break;
                    case "stairs":
                        floor.Squares.AddLast(new StairsSaveData(suffix) { Placement = p });
                        break;
                    case "play":
                        floor.Squares.AddLast(new StartSaveData() { Placement = p, SquareState = SquareState.UnFocus });
                        break;
                    case "cemetery":
                        floor.Squares.AddLast(new CemeterySaveData() { Placement = p });
                        break;
                    case "gold":
                        floor.Squares.AddLast(new GuineasSaveData(v) { Placement = p });
                        break;
                    case "totem":
                        floor.Squares.AddLast(new TotemSaveData() { Placement = p });
                        break;
                    default:
                        Debug.Log($"Unknown {prefix} {worksheet.Name} {p}");
                        break;
                }
            }

            return floor;
        }

        private static void EnemyLegalCheck(string id)
        {
            if (EnemyManager.Instance.EnemyBps.ContainsKey(id) == false)
            {
                Debug.LogError($"{id} not found");
            }
        }
    }
}


#endif