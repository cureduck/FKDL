using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "SO/Enemy", fileName = "Enemy")]
    public class EnemyBp : SerializedScriptableObject
    {
        public Texture Icon;
        public string Id;
        public BattleStatus Status;

        public string[] Skills;
    }
}