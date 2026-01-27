using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Configuration.Converters;
using UnityEditor;
using UnityEngine;

namespace Game.Core.Configuration.Editor
{
    public class ConfigEditorWindow : EditorWindow
    {
        private ConfigService _configService;
        private string _searchQuery = "";
        private Vector2 _scrollPosition;

        private Dictionary<string, bool> _foldouts = new();
        private GUIStyle _overrideStyle;
        private GUIStyle _keyStyle;

        [MenuItem("Window/Game/Config Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<ConfigEditorWindow>();
            window.titleContent = new GUIContent("Config Editor");
            window.Show();
        }

        public void SetConfigService(ConfigService configService)
        {
            _configService = configService;
            Repaint();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            ConfigServiceLocator.OnInstanceChanged += OnConfigServiceChanged;

            if (ConfigServiceLocator.HasInstance)
            {
                _configService = ConfigServiceLocator.Instance;
            }
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            ConfigServiceLocator.OnInstanceChanged -= OnConfigServiceChanged;
        }

        private void OnConfigServiceChanged(ConfigService service)
        {
            _configService = service;
            Repaint();
        }

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                _configService = null;
            }

            Repaint();
        }

        private void OnGUI()
        {
            InitStyles();

            if (_configService == null)
            {
                EditorGUILayout.HelpBox(
                    "Config Service not available. Enter Play mode to edit configs.",
                    MessageType.Info);
                return;
            }

            DrawToolbar();
            DrawConfigTree();
        }

        private void InitStyles()
        {
            _overrideStyle ??= new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold
            };

            _keyStyle ??= new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft
            };
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            EditorGUILayout.LabelField("Search:", GUILayout.Width(50));
            _searchQuery = EditorGUILayout.TextField(_searchQuery, EditorStyles.toolbarSearchField);

            if (GUILayout.Button("Reset All", EditorStyles.toolbarButton, GUILayout.Width(70)))
            {
                _configService.ResetAll();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawConfigTree()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            var keys = _configService.GetAllKeys()
                .Where(k => string.IsNullOrEmpty(_searchQuery) ||
                           k.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase))
                .OrderBy(k => k)
                .ToList();

            var tree = BuildTree(keys);
            DrawNode(tree, 0);

            EditorGUILayout.EndScrollView();
        }

        private TreeNode BuildTree(List<string> keys)
        {
            var root = new TreeNode { Name = "root", FullKey = "" };

            foreach (var key in keys)
            {
                var parts = key.Split('.');
                var current = root;

                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];
                    var isLeaf = i == parts.Length - 1;

                    if (!current.Children.TryGetValue(part, out var child))
                    {
                        child = new TreeNode
                        {
                            Name = part,
                            FullKey = string.Join(".", parts.Take(i + 1)),
                            IsLeaf = isLeaf
                        };
                        current.Children[part] = child;
                    }

                    current = child;
                }
            }

            return root;
        }

        private void DrawNode(TreeNode node, int indent)
        {
            foreach (var child in node.Children.Values.OrderBy(c => c.IsLeaf).ThenBy(c => c.Name))
            {
                if (child.IsLeaf)
                {
                    DrawLeaf(child, indent);
                }
                else
                {
                    DrawFolder(child, indent);
                }
            }
        }

        private void DrawFolder(TreeNode node, int indent)
        {
            if (!_foldouts.ContainsKey(node.FullKey))
                _foldouts[node.FullKey] = true;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indent * 15);

            _foldouts[node.FullKey] = EditorGUILayout.Foldout(
                _foldouts[node.FullKey],
                node.Name,
                true);

            EditorGUILayout.EndHorizontal();

            if (_foldouts[node.FullKey])
            {
                DrawNode(node, indent + 1);
            }
        }

        private void DrawLeaf(TreeNode node, int indent)
        {
            var key = node.FullKey;
            var isOverridden = _configService.IsOverridden(key);
            var valueType = _configService.GetValueType(key);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indent * 15);

            // Key label
            var style = isOverridden ? _overrideStyle : _keyStyle;
            EditorGUILayout.LabelField(node.Name, style, GUILayout.Width(150));

            // Value field
            DrawValueField(key, valueType);

            // Override indicator
            if (isOverridden)
            {
                GUI.color = new Color(1f, 0.7f, 0.3f);
                if (GUILayout.Button("R", GUILayout.Width(20)))
                {
                    _configService.ResetValue(key);
                }
                GUI.color = Color.white;
            }
            else
            {
                GUILayout.Space(24);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawValueField(string key, Type type)
        {
            var stringValue = _configService.GetStringValue(key);

            EditorGUI.BeginChangeCheck();
            string newValue = null;

            try
            {
                if (type == typeof(float))
                {
                    var parsed = ConfigConverterRegistry.StringToValue<float>(stringValue);
                    var result = EditorGUILayout.FloatField(parsed);
                    newValue = ConfigConverterRegistry.ValueToString(result);
                }
                else if (type == typeof(int))
                {
                    var parsed = ConfigConverterRegistry.StringToValue<int>(stringValue);
                    var result = EditorGUILayout.IntField(parsed);
                    newValue = ConfigConverterRegistry.ValueToString(result);
                }
                else if (type == typeof(bool))
                {
                    var parsed = ConfigConverterRegistry.StringToValue<bool>(stringValue);
                    var result = EditorGUILayout.Toggle(parsed);
                    newValue = ConfigConverterRegistry.ValueToString(result);
                }
                else if (type == typeof(string))
                {
                    newValue = EditorGUILayout.TextField(stringValue);
                }
                else if (type == typeof(Vector2))
                {
                    var parsed = ConfigConverterRegistry.StringToValue<Vector2>(stringValue);
                    var result = EditorGUILayout.Vector2Field("", parsed);
                    newValue = ConfigConverterRegistry.ValueToString(result);
                }
                else if (type == typeof(Vector3))
                {
                    var parsed = ConfigConverterRegistry.StringToValue<Vector3>(stringValue);
                    var result = EditorGUILayout.Vector3Field("", parsed);
                    newValue = ConfigConverterRegistry.ValueToString(result);
                }
                else if (type == typeof(Color))
                {
                    var parsed = ConfigConverterRegistry.StringToValue<Color>(stringValue);
                    var result = EditorGUILayout.ColorField(parsed);
                    newValue = ConfigConverterRegistry.ValueToString(result);
                }
                else
                {
                    newValue = EditorGUILayout.TextField(stringValue);
                }
            }
            catch
            {
                newValue = EditorGUILayout.TextField(stringValue);
            }

            if (EditorGUI.EndChangeCheck() && newValue != null && newValue != stringValue)
            {
                _configService.SetValueFromString(key, newValue);
            }
        }

        private class TreeNode
        {
            public string Name;
            public string FullKey;
            public bool IsLeaf;
            public Dictionary<string, TreeNode> Children = new();
        }
    }
}
