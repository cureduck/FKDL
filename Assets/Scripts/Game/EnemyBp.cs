using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "SO/Enemy", fileName = "Enemy")]
    public class EnemyBp : SerializedScriptableObject
    {
        public Sprite Icon;
        public string Id;

        public int Spirit;

        public string Description;

        public Rank Rank;
        public List<BuffData> Buffs;


        public SkillData[] Skills;
        public BattleStatus Status;
    }
}