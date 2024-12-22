using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arch.Core;
using Arch.Core.Extensions;
using DataForge.Editor.Reflection;
using DataForge.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DataForge.Editor
{
    public class EntityHierarchyWindow : EditorWindow
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        private VisualElement _detailsContainer;
        private EntityHierarchy _entityHierarchy;
        private ListView _listView;

        public EntityHierarchyWindow()
        {
            titleContent = new GUIContent("Entity Hierarchy");
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnDestroy()
        {
            _entityHierarchy?.Dispose();
        }

        private void CreateGUI()
        {
            _entityHierarchy = new EntityHierarchy();
            _entityHierarchy.Entities.ItemsChanged = OnCollectionChanged;

            var splitView = new TwoPaneSplitView(0, 100, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(splitView);

            _listView = new ListView
            {
                itemsSource = _entityHierarchy.Entities,
                makeItem = () => new Label(),
                bindItem = (item, index) =>
                {
                    var entity = _entityHierarchy.Entities[index];
                    var label = (Label)item;
                    label.text = entity.GetName();
                },
                selectionType = SelectionType.Single
            };

            _listView.selectionChanged += OnSelectionChanged;
            _detailsContainer = new VisualElement();

            splitView.Add(_listView);
            splitView.Add(_detailsContainer);
        }

        private void OnInspectorUpdate()
        {
            Refresh();
        }

        private void OnCollectionChanged()
        {
            if (_entityHierarchy.Entities.Any() && _listView.selectedItem == null)
            {
                _listView.SetSelection(0);

                var entity = _entityHierarchy.Entities[0];
                Select(entity);
            }

            Refresh();
        }

        private void Refresh()
        {
            _listView?.RefreshItems();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            Clear();
        }

        private void Clear()
        {
            _entityHierarchy?.Clear();
            _detailsContainer?.Clear();
            _listView?.Clear();
            Refresh();
        }

        private void OnSelectionChanged(IEnumerable<object> items)
        {
            var item = items.FirstOrDefault();
            if (item == null)
            {
                _detailsContainer.Clear();
                return;
            }

            var entity = (Entity)item;
            Select(entity);
        }

        private void Select(Entity entity)
        {
            _detailsContainer.Clear();
            _detailsContainer.Add(new Label(entity.GetName()));

            var components = entity.GetAllComponents();
            foreach (var component in components)
            {
                if (component == null)
                {
                    continue;
                }

                var type = component.GetType();
                var parent = new Foldout
                {
                    text = type.Name,
                    value = true
                };
                _detailsContainer.Add(parent);

                var members = GetMembers(type);
                foreach (var member in members)
                {
                    var memberName = member.Name();
                    if (memberName.Contains("k__BackingField"))
                    {
                        continue;
                    }

                    var value = member.GetValue(component);

                    if (value is ICollection collection)
                    {
                        AddCollection(memberName, collection, parent);
                    }
                    else
                    {
                        AddValue(memberName, value, parent);
                    }
                }
            }
        }

        private static void AddValue(string memberName, object value, VisualElement parent)
        {
            var element = new VisualElement();
            element.style.flexDirection = new(FlexDirection.Row);

            var lblName = new Label($"{memberName}: ");
            var lblValue = new Label(value?.ToString());

            element.Add(lblName);
            element.Add(lblValue);
            parent.Add(element);
        }

        private static void AddCollection(string memberName, IEnumerable collection, VisualElement parent)
        {
            var f = new Foldout() { text = memberName };
            parent.Add(f);

            foreach (var c in collection)
            {
                var element = new VisualElement();
                element.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

                var lblValue = new Label(c.ToString());
                element.Add(lblValue);

                f.Add(element);
            }
        }

        private List<Member> GetMembers(Type type)
        {
            var list = new List<Member>();
            foreach (var field in GetFields(type))
            {
                list.Add(new FieldMember(field));
            }

            foreach (var property in GetProperties(type))
            {
                list.Add(new PropertyMember(property));
            }

            return list;
        }

        private List<PropertyInfo> GetProperties(Type type)
        {
            var list = type
                .GetProperties(Flags)
                .ToList();

            return list;
        }

        private List<FieldInfo> GetFields(Type type)
        {
            var list = type
                .GetFields(Flags)
                .ToList();

            return list;
        }

        [MenuItem("Tools/Entity Hierarchy")]
        public static void ShowExample()
        {
            var window = GetWindow<EntityHierarchyWindow>();
        }
    }
}