using System.Collections.Generic;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using UI;
using UnityEngine;

namespace Game
{
    public class Buff : CsvData
    {
        public Buff(string id, Rank rank, Sprite icon) : base(rank, id, icon)
        {
            
        }
    }
}