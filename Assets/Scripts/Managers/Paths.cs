using System.IO;
using UnityEngine;

namespace Managers
{
    public static class Paths
    {
        public static string SkillDataPath => Path.Combine(Application.streamingAssetsPath, "Skills.csv");
        public static string PotionDataPath => Path.Combine(Application.streamingAssetsPath, "Potions.csv");
        public static string BuffDataPath => Path.Combine(Application.streamingAssetsPath, "Buffs.csv");
        public static string CrystalDataPath => Path.Combine(Application.streamingAssetsPath, "Crystal.csv");
        public static string RelicDataPath => Path.Combine(Application.streamingAssetsPath, "Relics.csv");


        
        public static string _initPath = Path.Combine( Application.streamingAssetsPath, "PlayerData.json");
        public static string _savePath = Path.Combine( Application.persistentDataPath, "PlayerData.json");
    }
}