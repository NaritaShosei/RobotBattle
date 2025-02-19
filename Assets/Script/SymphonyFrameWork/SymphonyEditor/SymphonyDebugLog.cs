﻿using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SymphonyFrameWork.Debugger
{
    /// <summary>
    /// エディタ用のログを発行するクラス
    /// </summary>
    public static class SymphonyDebugLog
    {
#if UNITY_EDITOR
        private static string _logText = string.Empty;
#endif

        /// <summary>
        /// エディタ上でのみ出力されるデバッグログ
        /// </summary>
        /// <param name="text"></param>
        [Conditional("UNITY_EDITOR")]
        public static void DirectLog(string text)
        {
#if UNITY_EDITOR
            Debug.Log(text);
#endif
        }

        /// <summary>
        /// ログのテキストにメッセージを追加する
        /// </summary>
        /// <param name="text"></param>
        [Conditional("UNITY_EDITOR")]
        public static void AddText(string text)
        {
#if UNITY_EDITOR
            _logText += $"{text}\n";
#endif
        }

        /// <summary>
        /// 追加されたメッセージを削除する
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void ClearText()
        {
#if UNITY_EDITOR
            _logText = string.Empty;
#endif
        }

        /// <summary>
        /// 追加されたメッセージをログに出力する
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void TextLog()
        {
#if UNITY_EDITOR
            Debug.Log(_logText);
#endif
        }

        /// <summary>
        /// コンポーネントだった場合に警告を表示する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        [Conditional("UNITY_EDITOR")]
        public static void CheckComponentNull<T>(this T component) where T : Component
        {
#if UNITY_EDITOR
            if (component == null)
            {
                Debug.LogWarning($"The component {typeof(T).Name} of {component.name} is null.");
            }
#endif
        }

        [Obsolete("この機能は安全性が保障されていません。CheckComponentNullを使用してください")]
        public static bool IsComponentNotNull<T>(this T component) where T : Component
        {
            if (component == null) {
                Debug.LogWarning($"The component of type {typeof(T).Name} is null.");
                return false;
            }
            else {
                return true;
            }
        }
    }
}