using Managers;
using Newtonsoft.Json;
using Sirenix.Utilities;

namespace Game
{
    public struct PotionData
    {
        public string Id;
        public int Count;
        
        [JsonIgnore] public bool IsEmpty => Id.IsNullOrWhitespace();
        public Potion Bp => PotionManager.Instance.Lib[Id];
    }
}