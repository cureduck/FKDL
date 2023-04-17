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
        public BuffType BuffType;
        public bool Stackable;
        [JsonIgnore] public Buff OppositeBp => BuffManager.Instance.TryGetById(OppositeId, out var op) ? op : null;
        
        public Buff(string id, Rank rank, bool stackable, Sprite icon, BuffType buffType, string oppositeId) : base(rank, id, icon)
        {
            OppositeId = oppositeId;
            BuffType = buffType;
            Stackable = stackable;
        }
    }
    
    public enum BuffType
    {
        Positive,
        Negative,
        Bless,
        Curse
    }
}