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
        public string OppositeId;
        public bool Positive;
        [JsonIgnore] public Buff OppositeBp => BuffManager.Instance.TryGetById(OppositeId, out var op) ? op : null;
        
        public Buff(string id, Rank rank, Sprite icon, bool positive, string oppositeId) : base(rank, id, icon)
        {
            OppositeId = oppositeId;
            Positive = positive;
        }
    }
}