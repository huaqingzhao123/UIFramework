using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using System.IO;
using System.Linq;
using System;
using WingjoyFramework.UIFramework.Runtime.Configs;
using WingjoyFramework.UIFramework.Runtime.Utility;

namespace WingjoyFramework.UIFramework.Editor
{
    public class ViewsConfigEditor : OdinMenuEditorWindow
    {
        [MenuItem("Tools/UI配置编辑器", false)]
        private static void OpenWindow()
        {
            var window = GetWindow<ViewsConfigEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1200, 600);
            window.titleContent = new GUIContent("UI配置编辑器");
            window.DrawUnityEditorPreview = true;
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true)
            {
                {"Views", ViewConfig.Instance, EditorIcons.Table},
            };
            var customMenuStyle = new OdinMenuStyle
            {
                BorderPadding = 0f,
                AlignTriangleLeft = true,
                TriangleSize = 16f,
                TrianglePadding = 0f,
                Offset = 20f,
                Height = 23,
                IconPadding = 0f,
                BorderAlpha = 0.323f
            };
            tree.DefaultMenuStyle = customMenuStyle;
            tree.Config.DrawScrollView = true;
            tree.Config.DrawSearchToolbar = true;

            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                //if (SirenixEditorGUI.ToolbarButton(new GUIContent("创建UI配置")))
                //{
                //    ViewConfigCreator.ShowDialog<ViewConfig>(Application.dataPath + "/Resource/Configs/Views", config =>
                //    {
                //        base.TrySelectMenuItemWithObject(config);
                //    });
                //}

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("刷新")))
                {
                    this.ForceMenuTreeRebuild();
                }
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }

    //public static class ViewConfigCreator
    //{
    //    public static void ShowDialog<T>(string defaultDestinationPath, Action<T> onConfigCreated = null) where T : ViewConfig
    //    {
    //        var root = defaultDestinationPath;
    //        defaultDestinationPath = EditorUtility.SaveFilePanel("Save file as", defaultDestinationPath, "New View", "json");
    //        var view = new ViewConfig();
    //        view.viewConfigs = new List<ViewConfig.UIConfig>();
    //        JsonHelper.SerializeToJson<ViewConfig>(view, defaultDestinationPath);
    //        AssetDatabase.Refresh();
    //    }
    //}
}


