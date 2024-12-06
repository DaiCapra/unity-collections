using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace AsmdefTool.Editor
{
    public class AsmdefToolEditorWindow : EditorWindow
    {
        public VisualTreeAsset visualTreeAsset;
        public string selection = string.Empty;
        public string rootName = string.Empty;
        public bool runtime;
        public bool editor;
        public bool editorTests;
        public bool playTests;

        public string outputText = string.Empty;
        public string foo;

        public void CreateGUI()
        {
            visualTreeAsset.CloneTree(rootVisualElement);

            var currentFolderField = rootVisualElement.Q<TextField>("currentFolderField");
            var rootNameField = rootVisualElement.Q<TextField>("rootNameField");

            var toggleRuntime = rootVisualElement.Q<Toggle>("toggleRuntime");
            var toggleEditor = rootVisualElement.Q<Toggle>("toggleEditor");
            var toggleEditorTest = rootVisualElement.Q<Toggle>("toggleEditorTests");
            var togglePlayTest = rootVisualElement.Q<Toggle>("togglePlayTests");

            var outputLabel = rootVisualElement.Q<Label>("outputLabel");
            var generateButton = rootVisualElement.Q<Button>("generateButton");

            Bind(currentFolderField, this, nameof(FloatField.value), nameof(selection));
            Bind(rootNameField, this, nameof(FloatField.value), nameof(rootName));

            Bind(toggleRuntime, this, nameof(Toggle.value), nameof(runtime));
            Bind(toggleEditor, this, nameof(Toggle.value), nameof(editor));
            Bind(toggleEditorTest, this, nameof(Toggle.value), nameof(editorTests));
            Bind(togglePlayTest, this, nameof(Toggle.value), nameof(playTests));

            Bind(outputLabel, this, nameof(Label.text), nameof(outputText));
            generateButton.clickable = new Clickable(OnGenerate);
        }

        private void OnGenerate()
        {
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
            var binding = new DataBinding()
            {
                bindingMode = mode
            };

            visualElement.SetBinding(propertyName, binding);
        }
    }
}