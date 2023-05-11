using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Game;
using I2.Loc;
using Managers;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Tools
{
    public static class Tools
    {
        public static T[] ChooseRandom<T>(int count, IList<T> list, Random random)
        {
            var s = new T[count];
            int[] selectNumArray =
                Enumerable.Range(0, list.Count).OrderBy(t => random.Next(10000)).Take(count).ToArray();
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = list.ToList()[selectNumArray[i]];
            }

            return s;
        }

        public static T ChooseRandom<T>(this IList<T> list, Random random) where T : class
        {
            var L = ChooseRandom<T>(1, list, random);
            if (L.Length > 0)
            {
                return L[0];
            }
            else
            {
                return null;
            }
        }

        public static T ChooseRandom<T>(this IEnumerable<T> list, Random random)
        {
            var index = random.Next(0, list.Count());
            return list.ElementAt(index);
        }


        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector,
            Random random)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = (float)random.NextDouble() * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence
                     select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;
            }

            return default(T);
        }


        /// <summary>
        /// 计算默认<>号内的数学表达式
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pair">标识计算段落的左右标识符，默认为<></param>
        /// <returns></returns>
        public static string Calculate(this string s, (char left, char right) pair = default)
        {
            if (pair == default) pair = ('<', '>');
            var pattern = $"{pair.left}.+?{pair.right}";
            var match = Regex.Match(s, pattern);
            var ca = Regex.Replace(match.Value, "[<>]", "");
            object result;
            try
            {
                result = new DataTable().Compute(ca, "");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                result = match.Value;
            }

            return Regex.Replace(s, pattern, result.ToString());
        }


        /// <summary>
        /// 移除默认<>号内的数学表达式
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pair"></param>
        /// <returns></returns>
        public static string RemoveBetween(this string s, (char left, char right) pair = default)
        {
            if (pair == default) pair = ('<', '>');
            var pattern = $"{pair.left}.+?{pair.right}";
            return Regex.Replace(s, pattern, "");
        }

        public static string RemoveBetween(this string s, (string left, string right) pair = default)
        {
            var pattern = $"{pair.left}.+?{pair.right}";
            return Regex.Replace(s, pattern, "");
        }


        public static void SetLocalizeParam(this Localize localize, IEnumerable<string> param,
            IEnumerable<string> values)
        {
            foreach (var (s1, s2) in param.Zip(values, (s1, s2) => (s1, s2)))
            {
                localize.GetComponent<LocalizationParamsManager>().SetParameterValue(s1, s2);
            }
        }

        public static void SetLocalizeParam(this Localize localize, string param, string values)
        {
            localize.GetComponent<LocalizationParamsManager>().SetParameterValue(param, values);
        }


        public static void SetLocalizeParam(this Localize localize, IDictionary<string, string> dictionary)
        {
            foreach (var pair in dictionary)
            {
                localize.GetComponent<LocalizationParamsManager>().SetParameterValue(pair.Key, pair.Value);
            }
        }

        public static void Calculate(this Localize localize)
        {
            localize.GetComponent<TMP_Text>().text = localize.GetComponent<TMP_Text>().text.Calculate();
        }

        public static void RemoveBetween(this Localize localize, (char left, char right) pair = default)
        {
            if (pair == default) pair = ('<', '>');
            localize.GetComponent<TMP_Text>().text = localize.GetComponent<TMP_Text>().text.RemoveBetween(pair);
        }

        public static void RemoveBetween(this Localize localize, (string left, string right) pair)
        {
            localize.GetComponent<TMP_Text>().text = localize.GetComponent<TMP_Text>().text.RemoveBetween(pair);
        }

        public static string ToString(this Rank @rank, RankDescType desc = RankDescType.Quality)
        {
            switch (desc)
            {
                case RankDescType.Size:
                    switch (rank)
                    {
                        case Rank.Normal:
                            return "small";
                        case Rank.Uncommon:
                            return "medium";
                        case Rank.Rare:
                            return "large";
                        case Rank.Ultra:
                            break;
                        case Rank.Prof:
                            break;
                        case Rank.God:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(rank), rank, null);
                    }

                    break;
                case RankDescType.Quality:
                    return rank.ToString();
                case RankDescType.Key:
                    switch (rank)
                    {
                        case Rank.Normal:
                            return "copper";
                            break;
                        case Rank.Uncommon:
                            return "silver";
                            break;
                        case Rank.Rare:
                            return "gold";
                            break;
                        default:
                            return "";
                            break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(desc), desc, null);
            }

            return null;
        }

        public static Texture2D ToTexture2D(this Sprite sprite)
        {
            var texture2D = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height);
            texture2D.SetPixels(pixels);
            texture2D.Apply();
            return texture2D;
        }
    }

    public static class RandomExtensions
    {
        public static RandomState Save(this Random random)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var temp = new MemoryStream())
            {
                binaryFormatter.Serialize(temp, random);
                return new RandomState(temp.ToArray());
            }
        }

        public static Random Restore(this RandomState state)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var temp = new MemoryStream(state.State))
            {
                return (Random)binaryFormatter.Deserialize(temp);
            }
        }
    }

    public struct RandomState
    {
        public readonly byte[] State;

        public RandomState(byte[] state)
        {
            State = state;
        }

        public override string ToString()
        {
            return State.ToString();
        }
    }

    public static class SkillExtensions
    {
        public static bool IsMainProf(this Skill skill)
        {
            return skill.Prof == GameDataManager.Instance.SecondaryData.Profs[0];
        }
    }
}