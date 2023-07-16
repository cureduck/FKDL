using System.Collections.Generic;
using System.Text;

namespace Game
{
    public static class StringDefines
    {
        private static StringBuilder stringBuilder = new StringBuilder();

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

        public static string GetKeyWordDescribe(string[] curKeyword, out string[] dataGroup)
        {
            List<string> curCanUseKeyword = new List<string>();

            for (int i = 0; i < curKeyword.Length; i++)
            {
                curCanUseKeyword.Add(curKeyword[i]);
                curCanUseKeyword.Add(KeywordDecorate(curKeyword[i]));
                ////curKey.Add(StringDefines.KeywordDecorate(skill.Keywords[i]));
                //if (StringDefines.KeywordsSet.Contains(skill.Keywords[i]))
                //{
                //    curKey.Add(StringDefines.KeywordDecorate(skill.Keywords[i]));
                //}
            }

            dataGroup = curCanUseKeyword.ToArray();
            stringBuilder.Clear();
            //Debug.Log(skill.Keywords.Length);
            for (int i = 0; i < curCanUseKeyword.Count; i++)
            {
                if (i % 2 == 0)
                {
                    if (i == 0)
                    {
                        stringBuilder.Append($"<color=yellow>{{[P{i + 1}]}}</color>:");
                    }
                    else
                    {
                        stringBuilder.Append($"\n<color=yellow>{{[P{i + 1}]}}</color>:");
                    }
                }
                else
                {
                    stringBuilder.Append($"{{[P{i + 1}]}}");
                }
            }

            return stringBuilder.ToString();
        }
    }
}