using AFramework.ResModule.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Path = System.IO.Path;

namespace AFramework.ResModule.Editor.Builder.View
{
    public class BuildPanel : EditorWindow
    {
        private string DefaultResPath = "BundleResources";
        private BuildSO _buildSo;
        private ReorderableList _reorderableList;
        private SerializedObject _serializedObject;

        [MenuItem("AFramework/Builder/BuildPanel")]
        public static void Open()
        {
            BuildPanel window = GetWindow<BuildPanel>();
            window.titleContent = new GUIContent("BuildPanel");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }

        private void CreateGUI()
        {
            _buildSo = BuildSO.GetDefaultBuildSo();

            _serializedObject = new SerializedObject(_buildSo);
            _reorderableList = new ReorderableList(_serializedObject, _serializedObject.FindProperty("BuildFilters"),
                true, true, true, true);
            _reorderableList.drawHeaderCallback = OnListHeaderGUI;
            _reorderableList.drawElementCallback = OnListElementGUI;
        }

        private void OnListHeaderGUI(Rect rect)
        {
            EditorGUI.LabelField(rect, "BuildFilters");
        }

        private void OnListElementGUI(Rect rect, int index, bool isActive, bool isFocused)
        {
            //TODO uodo时显示刷新有延迟
            var element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            var activeRect = new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight);
            var pathRect = new Rect(rect.x + 25, rect.y, rect.width - 420, EditorGUIUtility.singleLineHeight);
            //  365
            var selectRect = new Rect(pathRect.xMax + 5, rect.y, 50, EditorGUIUtility.singleLineHeight);
            var filterRect = new Rect(selectRect.xMax + 5, rect.y, 150, EditorGUIUtility.singleLineHeight);
            var collectOption = new Rect(filterRect.xMax + 5, rect.y, 150, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(activeRect, element.FindPropertyRelative("Active"), GUIContent.none);
            EditorGUI.PropertyField(pathRect, element.FindPropertyRelative("Path"), GUIContent.none);
            if (GUI.Button(selectRect, EditorGUIUtility.IconContent("Folder Icon")))
            {
                var defaultRoot = PathUtility.CombinePaths(Application.dataPath, DefaultResPath); //TODO 路径统一管理
                string path = EditorUtility.OpenFolderPanel("Select Folder", defaultRoot, "");
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith(Application.dataPath))
                    {
                        path = path.Replace(Application.dataPath, "Assets");
                        element.FindPropertyRelative("Path").stringValue = path;
                    }
                    else
                    {
                        ShowNotification(new GUIContent("Please select a folder in the Assets."));
                    }
                }
            }

            EditorGUI.PropertyField(filterRect, element.FindPropertyRelative("Filter"), GUIContent.none);
            EditorGUI.PropertyField(collectOption, element.FindPropertyRelative("CollectOption"), GUIContent.none);
        }

        private void OnListAdd(ReorderableList list)
        {
            var buildFilters = _reorderableList.serializedProperty;
        }

        private ToolbarTab _currentTab = ToolbarTab.Filter;
        private readonly string[] _toolbarTexts = new string[] { "Filter", "Build" };

        private void OnGUI()
        {
            _currentTab = (ToolbarTab)GUILayout.Toolbar((int)_currentTab, _toolbarTexts);

            switch (_currentTab)
            {
                case ToolbarTab.Filter:
                    GUIFilter();
                    break;
                case ToolbarTab.Build:
                    GUIBuild();
                    break;
            }
        }

        private void GUIBuild()
        {
            if (_buildSo == null)
                return;

            _serializedObject.Update();
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("BuildParameter"));
            _serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("Build"))
            {
                var builder = new BuildinBuildPipeline();
                builder.DefaultBuild();
            }
        }

        private void GUIFilter()
        {
            if (_buildSo == null || _reorderableList == null)
                return;

            _serializedObject.Update();
            _reorderableList.DoLayoutList();
            _serializedObject.ApplyModifiedProperties();
        }

        private enum ToolbarTab
        {
            Filter,
            Build,
        }
    }
}