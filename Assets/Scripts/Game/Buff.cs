using System;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class Buff : CsvData
    {
        public BuffType BuffType;
        public string OppositeId;
        public bool Stackable;

        public Buff(string id, Rank rank, bool stackable, Sprite icon, BuffType buffType, string oppositeId) : base(
            rank, id, icon)
        {
            OppositeId = oppositeId;
            BuffType = buffType;
            Stackable = stackable;
        }

        [JsonIgnore] public Buff OppositeBp => BuffManager.Instance.TryGetById(OppositeId, out var op) ? op : null;
    }

    [Flags]
    public enum BuffType
    {
        Positive = 1 << 0,
        Negative = 1 << 1,
        Blessing = 1 << 2,
        Curse = 1 << 3,
        Buff = Positive | Negative
    }
}