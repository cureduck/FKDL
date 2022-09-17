using System.IO;
using UnityEngine;

namespace Managers
{
    public static class Paths
    {
        public static string SkillDataPath => Path.Combine(Application.streamingAssetsPath, "Skills.csv");
        public static string PotionDataPath => Path.Combine(Application.streamingAssetsPath, "Potions.csv");
    }
}