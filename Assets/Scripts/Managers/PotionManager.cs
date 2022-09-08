using System;
using System.Collections.Generic;
using Csv;
using Game;
using Sirenix.OdinInspector;
using System.IO;
using UnityEngine;

namespace Managers
{
    public class PotionManager : Singleton<PotionManager>
    {
        public Dictionary<string, Potion> Potions;

        private void Start()
        {
            Load();
        }


        private void Load()
        {
            Potions = new Dictionary<string, Potion>();
            var csv = File.ReadAllText(Paths.PotionDataPath);


            foreach (var line in CsvReader.ReadFromText(csv))
            {
                try
                {
                    var potion = Line2Potion(line);
                    Potions[potion.Id] = potion;
                }
                catch (Exception)
                {
                    Debug.Log("potion load failed");
                }
            }
        }


        private static Potion Line2Potion(ICsvLine line)
        {
            return new Potion
            {
                Id = line[0],
                Rank = (Rank) int.Parse(line[1]),
                Param1 = int.Parse(line[2] == ""?line[2]:"0"),
                Param2 = int.Parse(line[3] == ""?line[3]:"0"),
            };
        }
    }
}