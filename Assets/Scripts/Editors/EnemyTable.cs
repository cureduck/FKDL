#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using Game;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    public class EnemyTable
    {
        [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
        private readonly List<EnemyWrapper> allEnemies;

        public EnemyBp this[int index] => allEnemies[index].EnemyData;

        public EnemyTable(IEnumerable<EnemyBp> enemies)
        {
            this.allEnemies = enemies.Select(x => new EnemyWrapper(x)).ToList();
        }
        
        private class EnemyWrapper
        {
            public EnemyBp EnemyData { get; }

            public EnemyWrapper(EnemyBp data)
            {
                this.EnemyData = data;
            }
            
            [TableColumnWidth(50, false)]
            [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center)]
            public Sprite Icon { get { return this.EnemyData.Icon; } set { this.EnemyData.Icon = value; EditorUtility.SetDirty(this.EnemyData); } }
            
        }
    }
}

#endif