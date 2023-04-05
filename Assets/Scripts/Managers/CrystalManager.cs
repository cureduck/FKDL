using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Csv;
using Game;
using Tools;
using UnityEngine;

namespace Managers
{
    public class CrystalManager : Singleton<CrystalManager>
    {
        public CustomDictionary<Crystal> Lib;
        protected string CsvPath => Paths.CrystalDataPath;

        private void Start()
        {
            Load();
        }

        protected void Load()
        {
            Lib = new CustomDictionary<Crystal>();
            var csv = File.ReadAllText(CsvPath);

            foreach (var line in CsvReader.ReadFromText(csv))
            {
                try
                {
                    HandleLine(line);
                }
                catch (Exception)
                {
                    Debug.Log($" {line["id"]} crystal load failed");
                }
            }
        }


        private void HandleLine(ICsvLine line)
        {
            var id = line["id"];//.ToLower().Replace(" ", "");
            var rank = (Rank) int.Parse(line["rank"]);
            var isTitle = line["type"].Equals("title");

            if (isTitle)
            {
                Lib[id] = new Crystal(rank)
                {
                    Id = id,
                    Title = id
                };
            }
            else
            {
                var effects = line["effects"];
                var tit = id.Split('_')[0];
                var suf = id.Split('_')[1];
                CostType costType;
                switch (line["costtype"])
                {
                    case "hp":
                        costType = CostType.Hp;
                        break;
                    case "mp":
                        costType = CostType.Mp;
                        break;
                    case "gold":
                        costType = CostType.Gold;
                        break;
                    default:
                        costType = CostType.Hp;
                        break;
                }
                
                var opt = new Crystal.Option()
                {
                    Line = suf,
                    Effect = effects,
                    CostInfo = new CostInfo()
                    {
                        Value = int.TryParse(line["cost"], out var o) ? o : 0,
                        CostType = costType
                            
                    }
                };

                Lib[tit].Options.Add(opt);

            }
        }
    }
}