using System.Collections.Generic;
using System.Text;

namespace Game
{
    public static class StringDefines
    {
        private static StringBuilder stringBuilder = new StringBuilder();
        private static HashSet<string> _keywordsSet;

        public static HashSet<string> KeywordsSet
        {
            get
            {
                if (_keywordsSet == null)
                {
                    _keywordsSet = new HashSet<string>();
                    KeywordDataFile keywordDataFile = KeywordDataFile.Instarnce;
                    for (int i = 0; i < keywordDataFile.curKeyWord.Length; i++)
                    {
                        _keywordsSet.Add(keywordDataFile.curKeyWord[i]);
                    }
                }

                return _keywordsSet;
            }
        }

        private static string KeywordDecorate(string keyword)
        {
            return $"Keyword_{keyword}_Desc";
        }

        public static string GetKeyWordDescribe(string[] curKeyword, out string[] dataGroup)
        {
            List<string> curCanUseKeyword = new List<string>();

            for (int i = 0; i < curKeyword.Length; i++)
            {
                if (string.IsNullOrEmpty(curKeyword[i])) continue;
                if (!KeywordsSet.Contains(curKeyword[i])) continue;
                curCanUseKeyword.Add($"Keyword_{curKeyword[i]}_Title");
                curCanUseKeyword.Add(KeywordDecorate(curKeyword[i]));
                ////curKey.Add(StringDefines.KeywordDecorate(skill.Keywords[i]));
                //if (StringDefines.KeywordsSet.Contains(skill.Keywords[i]))
                //{
                //    curKey.Add(StringDefines.KeywordDecorate(skill.Keywords[i]));
                //}
            }

            dataGroup = curCanUseKeyword.ToArray();
            stringBuilder.Clear();
            if (curCanUseKeyword.Count == 0) return string.Empty;
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