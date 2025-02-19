﻿using SymphonyFrameWork.Editor;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Config
{
    public partial class SceneManagerConfig
    {
        public void OnEnable()
        {
            _sceneList = EditorBuildSettings.scenes
                .Select(s => Path.GetFileNameWithoutExtension(s.path))
                .ToArray();
        }
    }

    /// <summary>
    /// Enum生成用のボタンを実行
    /// </summary>
    [CustomEditor(typeof(SceneManagerConfig))]
    public class MyScriptEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);
            var manager = target as SceneManagerConfig;

            if (GUILayout.Button("SceneListを読み込む"))
            {
                manager.OnEnable();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Enumを生成する"))
            {
                EnumGenerator.EnumGenerate(manager.SceneList, nameof(manager.SceneList));
            }
        }
    }
}
