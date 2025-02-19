﻿using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SymphonyFrameWork.Utility
{
    /// <summary>
    /// Taskの機能を拡張するクラス
    /// </summary>
    public static class SymphonyTask
    {
        /// <summary>
        /// バッググラウンドで処理する
        /// </summary>
        /// <param name="action"></param>
        public static async void BackGroundThreadAction(Action action)
        {
            await Awaitable.BackgroundThreadAsync();
            action.Invoke();
            Debug.Log($"{action.Method} is done");
        }

        /// <summary>
        /// バッググラウンドで処理する
        /// </summary>
        /// <param name="action"></param>
        public static async Task BackGroundThreadActionAsync(Action action)
        {
            await Awaitable.BackgroundThreadAsync();
            action.Invoke();
            Debug.Log($"{action.Method} is done");

            return;
        }

        /// <summary>
        /// 条件がtrueになるまで待機する
        /// </summary>
        /// <param name="action">条件の結果を返すメソッド</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task WaitUntil(Func<bool> action, CancellationToken token = default)
        {
            while (!action.Invoke())
            {
                await Awaitable.NextFrameAsync(token);
            }
        }
    }
}