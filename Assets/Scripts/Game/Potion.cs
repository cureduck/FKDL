﻿using Managers;
using UnityEngine;

namespace Game
{
    public class Potion : CsvData, IKeyword
    {
        public float Param1;
        public float Param2;
        public string Upgrade;

        public Potion(Rank rank, string id, string upgrade, string[] keywords, Sprite icon) : base(rank, id, icon)
        {
            Keywords = keywords;
            Upgrade = upgrade;
        }

        public Potion(Rank rank, string id) : base(rank, id)
        {
        }

        public string Des
        {
            get { return $"{Id}_desc"; }
        }

        public string[] Keywords { get; }
    }
}