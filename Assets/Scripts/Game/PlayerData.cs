using Game;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class PlayerData : SaveData
    {
        public string Id;
        public SkillData[] Skills;

        public BattleStatus BattleStatus;
        public PlayerStatus PlayerStatus;

        private static readonly string _initPath = Application.dataPath + "/Resources/PlayerInit/PlayerData.json";
        private static readonly string _savePath = Application.persistentDataPath + "/PlayerData.json";

        public void Save()
        {
            Save(_savePath);
        }

        public static PlayerData LoadFromInit()
        {
            return Load(_initPath);
        }

        public static PlayerData LoadFromSave()
        {
            return Load(_savePath);
        }
        
        
        [Button]
        public static PlayerData Load(string path)
        {
            return Load<PlayerData>(path);
        }
    }
}