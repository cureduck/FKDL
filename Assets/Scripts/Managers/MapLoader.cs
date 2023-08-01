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
                Debug.Log("map count : " + excel.Workbook.Worksheets.Count);

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
                    var n = Path.GetFileNameWithoutExtension(file);
                    var t = n.Replace("test", "");
                    if (int.Parse(t) > max)
                    {
                        max = int.Parse(t);
                    }
                }
            }

            DeletePrevious();

            Load($"test{max}");
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

                var square = CreateSingleSquare(prefix, suffix, p, GetFloorLevel(floor.FloorName));

                if (square == null) continue;
                floor.Squares.AddLast(square);
            }

            return floor;
        }


        private static int GetFloorLevel(string floorName)
        {
            if (floorName.StartsWith("A"))
            {
                return int.Parse(floorName.Substring(1, 1));
            }
            else
            {
                return int.Parse(floorName);
            }
        }


        private static MapData CreateSingleSquare(string prefix, string suffix, Placement p, int floorLevel = 0)
        {
            MapData toReturn = null;

            int.TryParse(suffix, out var v);

            switch (prefix)
            {
                case "treasure":
                    toReturn = new ChestSaveData((Rank)v) { Placement = p };
                    break;
                case "casino":
                    toReturn = new CasinoSaveData() { Placement = p };
                    break;
                case "grassland":
                    toReturn = new SupplySaveData(SupplyType.Grassland) { Placement = p };
                    break;
                case "spring":
                    toReturn = new SupplySaveData(SupplyType.Spring) { Placement = p };
                    break;
                case "camp":
                    toReturn = new SupplySaveData(SupplyType.Camp) { Placement = p };
                    break;
                case "shop":
                    toReturn = new ShopSaveData() { Placement = p };
                    break;
                case "door":
                    toReturn = new DoorSaveData((Rank)v) { Placement = p };
                    break;
                case "key":
                    toReturn = new KeySaveData((Rank)v) { Placement = p };
                    break;
                case "crystal":
                    toReturn = new CrystalSaveData((Rank)v) { Placement = p };
                    break;
                case "traveler":
                    toReturn = new TravellerSaveData(suffix) { Placement = p };
                    break;
                case "enemy":
                    toReturn = new EnemySaveData(suffix.ToLower()) { Placement = p };
                    //EnemyLegalCheck(suffix.ToLower());
                    break;
                case "rock":
                    toReturn = new RockSaveData(floorLevel) { Placement = p };
                    break;
                case "obsidian":
                    toReturn = new ObsidianSaveData() { Placement = p };
                    break;
                case "stairs":
                    toReturn = new StairsSaveData(suffix) { Placement = p };
                    break;
                case "play":
                    toReturn = new StartSaveData() { Placement = p, SquareState = SquareState.UnFocus };
                    break;
                case "cemetery":
                    toReturn = new CemeterySaveData() { Placement = p };
                    break;
                case "gold":
                    toReturn = new GuineasSaveData(floorLevel) { Placement = p };
                    break;
                case "totem":
                    toReturn = new TotemSaveData() { Placement = p };
                    break;
                case "special":
                    toReturn = new SpecialSaveData(suffix) { Placement = p };
                    break;
                default:
                    Debug.Log($"Unknown {prefix} {floorLevel} {p}");
                    break;
            }

            return toReturn;
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