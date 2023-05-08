using System.Linq;
using Game;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    public class EnemyOverView : GlobalConfig<EnemyOverView>
    {
        [ReadOnly] [ListDrawerSettings(Expanded = true)]
        public EnemyBp[] AllEnemies;

#if UNITY_EDITOR
        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        public void UpdateEnemyOverview()
        {
            this.AllEnemies = AssetDatabase.FindAssets("t:EnemyBp")
                .Select(guid => AssetDatabase.LoadAssetAtPath<EnemyBp>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }
#endif
    }
}