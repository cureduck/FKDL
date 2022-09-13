using System;
using System.Collections.Generic;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Game
{
    public class EnemySaveData : FighterData
    {
        public string Id;

        public EnemySaveData(string id) : base()
        {
            Id = id;
            Status = Bp.Status;
            Skills = new SkillData[Bp.Skills.Length];
            //Array.Copy(Bp.Skills, Skills, Bp.Skills.Length);
        }
        
        [JsonIgnore] public EnemyBp Bp => EnemyManager.Instance.EnemyBps[Id];
    }
}