﻿using SymphonyFrameWork.Config;
using SymphonyFrameWork.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    ///     SymphonyFrameWorkの管理パネルを表示するクラス
    /// </summary>
    public class SymphonyWindow : EditorWindow
    {
        private const string WINDOW_NAME = "Symphony Administrator";
        private const string UITK_PATH = SymphonyConstant.FRAMEWORK_PATH + "/SymphonyEditor/Editor/Administrator/UITK/";

        private PauseWindow _pauseWindow;
        private ServiceLocatorWindow _serviceLocatorWindow;
        
        private void OnEnable()
        {
            var container = LoadWindow();

            if (container != null)
            {
                _pauseWindow = container.Q<PauseWindow>();
                _serviceLocatorWindow = container.Q<ServiceLocatorWindow>();
                SceneLoaderInit(container);
            }
            else
            {
                Debug.LogWarning("ウィンドウがロードできませんでした");
            }
            
            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }
        
        private void Update()
        {
            _pauseWindow?.Update();
            _serviceLocatorWindow?.Update();
        }



        /// <summary>
        ///     ウィンドウ表示
        /// </summary>
        [MenuItem(SymphonyConstant.MENU_PATH + WINDOW_NAME, priority = 0)]
        public static void ShowWindow()
        {
            var wnd = GetWindow<SymphonyWindow>();
            wnd.titleContent = new GUIContent(WINDOW_NAME);
        }

        /// <summary>
        ///     UXMLを追加
        /// </summary>
        /// <returns></returns>
        private TemplateContainer LoadWindow()
        {
            rootVisualElement.Clear();
            
            var windowTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UITK_PATH + "SymphonyWindow.uxml");
            ;
            if (windowTree != null)
            {
                var windowElement = windowTree.Instantiate();
                rootVisualElement.Add(windowElement);
                return windowElement;
            }

            Debug.LogError("ウィンドウが見つかりません");
            return null;
        }

        #region SceneLoader

        private static Toggle _autoSceneListUpdateToggle;

        private static void SceneLoaderInit(VisualElement root)
        {
            //コンフィグデータを取得
            var config = SymphonyConfigLocator.GetConfig<AutoEnumGeneratorConfig>();
            _autoSceneListUpdateToggle = root.Q<Toggle>("enum-scene");
            _autoSceneListUpdateToggle.value = config.AutoSceneListUpdate;

            //トグルが変更された時にコンフィグを更新
            _autoSceneListUpdateToggle.RegisterValueChangedCallback(
                evt => config.AutoSceneListUpdate = evt.newValue);
        }

        #endregion
    }
}