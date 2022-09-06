using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace Editors
{
    public class FloorEditorWindow : OdinMenuEditorWindow
    {
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

            return tree;
        }
    }
}