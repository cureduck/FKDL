﻿using System;
using System.Collections.Generic;
using Csv;
using Game;
using Sirenix.OdinInspector;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class PotionManager : Singleton<PotionManager>
    {
        public Dictionary<string, Potion> Potions;

        public Dictionary<Rank, LinkedList<Potion>> OrderedPotions;
        
        private void Start()
        {
            Load();
        }


        private void Load()
        {
            Potions = new Dictionary<string, Potion>();
            var csv = File.ReadAllText(Paths.PotionDataPath);
            OrderedPotions = new Dictionary<Rank, LinkedList<Potion>>();

            foreach (var line in CsvReader.ReadFromText(csv))
            {
                try
                {
                    var potion = Line2Potion(line);
                    Potions[potion.Id] = potion;

                    if (!OrderedPotions.ContainsKey(potion.Rank))
                        OrderedPotions[potion.Rank] = new LinkedList<Potion>();
                    OrderedPotions[potion.Rank].AddLast(potion);
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
        
        
        public string[] Roll(Rank roll, int count)
        {
            var s = new string[count];
            int[] selectNumArray = Enumerable.Range(0, OrderedPotions[roll].Count).OrderBy(t => Guid.NewGuid()).Take(count).ToArray();
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = OrderedPotions[roll].ToList()[selectNumArray[i]].Id;
            }
            return s;
        }
        
    }
}