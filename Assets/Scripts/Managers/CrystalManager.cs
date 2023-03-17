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

        
        private void Start()
        {
            Load();
        }


        private void Load()
        {
            Lib = new CustomDictionary<Crystal>();
            var csv = File.ReadAllText(Paths.CrystalDataPath);

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
                Lib[id] = new Crystal()
                {
                    Id = id,
                    Rank = rank,
                    Title = id
                };
            }
            else
            {
                var effects = line["effects"];
                var tit = id.Split('_')[0];
                var suf = id.Split('_')[1];
                var opt = new Crystal.Option()
                {
                    Line = suf,
                    Effect = effects
                };

                Lib[tit].Options.Add(opt);

            }
        }
    }
}