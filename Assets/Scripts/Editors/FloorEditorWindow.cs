#if UNITY_EDITOR

using System.IO;
using System.Linq;
using Game;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    public class FloorEditorWindow : OdinMenuEditorWindow
    {
        private static string FloorPath => Path.Combine(Application.dataPath, "Resources", "Floors");


        [MenuItem("Tools/Editor/FloorEditor")]
        private static void Open()
        {
            var window = GetWindow<FloorEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28f;
            tree.Config.DrawSearchToolbar = true;

            tree.Add("Add New", new CreateNewFloor());

            return tree;
        }

        public class CreateNewFloor
        {
            public CreateNewFloor()
            {
                floor = new Map.Floor();
            }

            [ShowInInspector] public Map.Floor floor;

            [Button("Create Floor")]
            private void CreateFloor()
            {
                var f = JsonConvert.SerializeObject(floor, settings: new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
                var count = Directory.GetFiles(Path.Combine(FloorPath, floor.FloorName))
                    .Count(name => name.EndsWith(".json"));
                File.WriteAllText(Path.Combine(FloorPath, floor.FloorName, count + ".json"), f);
            }

            [Button("Load")]
            private void Load()
            {
                GameManager.Instance.LoadFloor(floor);
            }
        }
    }
}

#endif