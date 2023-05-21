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

    public enum BuffType
    {
        Positive,
        Negative,
        Blessing,
        Curse
    }
}