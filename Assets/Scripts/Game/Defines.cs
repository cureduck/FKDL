using System.Collections.Generic;

namespace Game
{
    public static class StringDefines
    {
        public static HashSet<string> KeywordsSet = new HashSet<string>()
        {
            "CounterCharge",
            "Engaging",
        };

        public static Dictionary<string, string> SoundEffectsSet = new Dictionary<string, string>()
        {
            { "magic_attack", "attack" }
        };

        public static string KeywordDecorate(string keyword)
        {
            return $"kw_{keyword}_desc";
        }
    }
}