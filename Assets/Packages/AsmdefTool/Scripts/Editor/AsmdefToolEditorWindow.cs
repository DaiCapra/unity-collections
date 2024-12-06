using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AsmdefTool.Editor
{
    public class AsmdefToolEditorWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;

        private string Selection { get; set; } = string.Empty;
        private string RootName { get; set; } = string.Empty;
        private bool Runtime { get; set; }
        private bool Editor { get; set; }
        private bool EditorTests { get; set; }
        private bool PlayTests { get; set; }

        public void CreateGUI()
        {
            visualTreeAsset.CloneTree(rootVisualElement);

            var currentFolderField = rootVisualElement.Q<TextField>("currentFolderField");
            // Bind(currentFolderField, this, "value", nameof(Selection));

            var rootNameField = rootVisualElement.Q<TextField>("rootNameField");
            rootNameField.dataSource = this;
            rootNameField.dataSourcePath = new PropertyPath(nameof(RootName));
            
            // Bind(rootNameField, this, "value", nameof(RootName));
            
            var toggleRuntime = rootVisualElement.Q<Toggle>("toggleRuntime");
            var toggleEditor = rootVisualElement.Q<Toggle>("toggleEditor");
            var toggleEditorTest = rootVisualElement.Q<Toggle>("toggleEditorTest");
            var togglePlayTest = rootVisualElement.Q<Toggle>("togglePlayTest");

            var outputLabel = rootVisualElement.Q<Label>("outputLabel");
            var generateButton = rootVisualElement.Q<Button>("generateButton");
        }

        [MenuItem("Tools/Asmdef Tool")]
        public static void ShowExample()
        {
            var window = GetWindow<AsmdefToolEditorWindow>();
            window.titleContent = new GUIContent("Asmdef Tool");
        }

        public void Bind(
            VisualElement visualElement,
            object source,
            string propertyName,
            string path,
            BindingMode mode = BindingMode.TwoWay
        )
        {
            visualElement.dataSource = source;
            visualElement.dataSourcePath = new PropertyPath(path);
            visualElement.SetBinding(propertyName, new DataBinding()
            {
                bindingMode = BindingMode.TwoWay
            });
        }
    }
}