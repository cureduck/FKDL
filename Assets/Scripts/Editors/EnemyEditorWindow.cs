﻿#if UNITY_EDITOR

using System.Linq;
using Game;
using Managers;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;


namespace Editors
{
    public class EnemyEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Editor/EnemyEditor")]
        private static void Open()
        {
            var window = GetWindow<EnemyEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }
        
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28f;
            tree.Config.DrawSearchToolbar = true;
            
            tree.Add("Add New", new CreateNewEnemy());
            
            EnemyOverView.Instance.UpdateEnemyOverview();
            tree.Add("Enemies", new EnemyTable(EnemyOverView.Instance.AllEnemies));
            tree.AddAllAssetsAtPath("Enemies", "Resources/EnemyBp", typeof(EnemyBp), true);

            tree.EnumerateTree().AddIcons<EnemyBp>(x => x.Icon);
            return tree;
        }
        
        
        public class CreateNewEnemy
        {
            public CreateNewEnemy()
            {
                enemyData = ScriptableObject.CreateInstance<EnemyBp>();
                enemyData.Id = "New Enemy Data";
            }
            
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public EnemyBp enemyData;

            [Button("Add New Enemy SO")]
            private void CreateNewData()
            {
                AssetDatabase.CreateAsset(enemyData, "Assets/Resources/EnemyBp/" + enemyData.Id +".asset");

                enemyData = ScriptableObject.CreateInstance<EnemyBp>();
                enemyData.Id = "NewEnemy";
            }
        }


        protected override void OnBeginDrawEditors()
        {
            //base.OnBeginDrawEditors();
            var selected = MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = MenuTree.Config.SearchToolbarHeight;

            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                if (SirenixEditorGUI.ToolbarButton("Delete Current"))
                {
                    var asset = MenuTree.Selection.SelectedValue as EnemyBp;
                    var path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
                
                SirenixEditorGUI.EndHorizontalToolbar();
            }
        }
    }
}

#endif