using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class Potion
    {
        [JsonIgnore] public Texture Icon;
        public string Id;
        public Rank Rank;
        public int Param1;
        public int Param2;
    }
}