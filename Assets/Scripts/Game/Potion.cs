using System.Collections.Generic;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class Potion : CsvData
    {
        public float Param1;

        public string Des
        {
            get { return $"{Id}_desc"; }
        }

        public Potion(Rank rank, string id, Sprite icon) : base(rank, id, icon)
        {
        }

        public Potion(Rank rank, string id) : base(rank, id)
        {
        }
    }
}