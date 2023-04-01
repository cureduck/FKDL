#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using Game;
using Sirenix.OdinInspector;
using Debug = UnityEngine.Debug;
using OfficeOpenXml;

namespace Managers
{
    public class MapLoader : Singleton<MapLoader>
    {

        [Button]
        void Load(string fileName)
        {
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


        public Map.Floor LoadFloor(ExcelWorksheet worksheet)
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

                if (int.TryParse(suffix, out var v));

                switch (prefix)
                {
                    case "treasure":
                        floor.Squares.AddLast(new ChestSaveData((Rank) v) {Placement = p});
                        break;
                    case "casino":
                        floor.Squares.AddLast(new CasinoSaveData() {Placement = p});
                        break;
                    case "grassland":
                        floor.Squares.AddLast(new SupplySaveData(SupplyType.Grassland) {Placement = p});
                        break;
                    case "spring":
                        floor.Squares.AddLast(new SupplySaveData(SupplyType.Spring) {Placement = p});
                        break;
                    case "camp":
                        floor.Squares.AddLast(new SupplySaveData(SupplyType.Camp) {Placement = p});
                        break;
                    case "shop":
                        floor.Squares.AddLast(new ShopSaveData() {Placement = p});
                        break;
                    case "door":
                        floor.Squares.AddLast(new DoorSaveData((Rank)v) {Placement = p});
                        break;
                    case "key":
                        floor.Squares.AddLast(new KeySaveData((Rank)v) {Placement = p});
                        break;
                    case "crystal":
                        floor.Squares.AddLast(new CrystalSaveData((Rank)v) {Placement = p});
                        break;
                    case "traveler":
                        floor.Squares.AddLast(new TravellerSaveData() {Placement = p});
                        break;
                    case "enemy":
                        floor.Squares.AddLast(new EnemySaveData(suffix.ToLower()) {Placement = p});
                        break;
                    case "rock":
                        floor.Squares.AddLast(new RockSaveData() {Placement = p});
                        break;
                    case "obsidian":
                        floor.Squares.AddLast(new ObsidianSaveData() {Placement = p});
                        break;
                    case "stairs":
                        floor.Squares.AddLast(new StairsSaveData(suffix) {Placement = p});
                        break;
                    case "play":
                        floor.Squares.AddLast(new StartSaveData() {Placement = p, SquareState = SquareState.UnFocus});
                        break;
                    case "cemetery":
                        floor.Squares.AddLast(new CemeterySaveData() {Placement = p});
                        break;
                    case "gold":
                        floor.Squares.AddLast(new GoldSaveData(v) {Placement = p});
                        break;
                    case "totem":
                        floor.Squares.AddLast(new TotemSaveData() {Placement = p});
                        break;
                    default:
                        Debug.Log($"Unknown {prefix} {worksheet.Name} {p}");
                        break;
                }

            }

            return floor;
        }

    }
}


#endif