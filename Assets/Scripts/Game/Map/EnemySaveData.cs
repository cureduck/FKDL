using System;
using System.Collections.Generic;
using Managers;
using Newtonsoft.Json;

namespace Game
{
    public class EnemySaveData : MapData
    {
        public int CurHp;
        public string Id;
        
        [JsonIgnore] public EnemyBp Bp => EnemyManager.Instance.EnemyBps[Id];
    }
}