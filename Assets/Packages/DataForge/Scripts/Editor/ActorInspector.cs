using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Arch.Core;
using Arch.Core.Extensions;
using DataForge.Entities;
using DataForge.Objects;
using DataForge.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace DataForge.Editor
{
    [CustomEditor(typeof(Actor), editorForChildClasses: true)]
    public class ActorInspector : UnityEditor.Editor
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private const int UpdateRate = 100;
        private Actor _actor;
        private VisualElement _root;

        private void OnEnable()
        {
            EditorApplication.update += UpdateInspector;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateInspector;
        }

        public override VisualElement CreateInspectorGUI()
        {
            _root ??= new();
            _actor = target as Actor;
            
            UpdateActor();
            return _root;
        }

        private void UpdateInspector()
        {
            var frame = Time.frameCount;
            if (frame % UpdateRate != 0)
            {
                return;
            }

            UpdateActor();
        }

        private void UpdateActor()
        {
            if (_actor == null || _actor.entity == Entity.Null)
            {
                return;
            }

            var entity = _actor.entity;
            var list = GetEntityData(entity);

            _root.Clear();
            _root.Add(new Label { text = $"Id: {entity.GetId()}" });
            _root.Add(new Label { text = $"Blueprint: {entity.GetBlueprintId()}" });

            foreach (var data in list)
            {
                var foldout = new Foldout { text = data.name };
                _root.Add(foldout);

                foreach (var kv in data.map)
                {
                    var value = kv.Value;
                    if (value is ICollection collection)
                    {
                        AddCollection(kv.Key, collection, foldout);
                    }
                    else
                    {
                        AddValue(kv.Key, kv.Value, foldout);
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
                element.style.flexDirection = new(FlexDirection.Row);

                var lblValue = new Label(c.ToString());
                element.Add(lblValue);

                f.Add(element);
            }
        }

        private List<ObjectData> GetEntityData(Entity entity)
        {
            var list = new List<ObjectData>();
            var components = entity.GetAllComponents();
            foreach (var component in components)
            {
                if (component == null)
                {
                    continue;
                }

                var type = component.GetType();

                var data = new ObjectData();
                data.type = type;
                data.name = type.Name;

                var members = GetMembers(type);
                foreach (var member in members)
                {
                    var key = member.Name();
                    if (key.Contains("backingfield", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var value = member.GetValue(component);
                    data.map[key] = value;
                }

                list.Add(data);
            }

            return list;
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
    }
}