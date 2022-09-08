using System.IO;
using UnityEngine;

namespace Managers
{
    public static class Paths
    {
        public static string SkillDataPath => Path.Combine(Application.dataPath, "GameData", "Skills", "Skills.csv");
        public static string PotionDataPath => Path.Combine(Application.dataPath, "GameData", "Potions", "Potions.csv");
    }
}