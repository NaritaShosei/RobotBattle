﻿using UnityEditor;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    /// SymphonyFrameWorkのディレクトリを保護するクラス
    /// </summary>
    public class SymphonyAssetPostProcessor : AssetPostprocessor
    {
        private const string LOCK_PATH = SymphonyConstant.MENU_PATH + "Symphony Asset Lock";

        static SymphonyAssetPostProcessor()
        {
            // Unityエディタが再起動された後でも状態が反映されるようにする
            EditorApplication.delayCall += () => ValidateLock();
        }

        /// <summary>
        /// メニューがクリックされたときにチェック状態を反転する
        /// </summary>
        [MenuItem(LOCK_PATH, priority = 200)]
        private static void ToggleOption()
        {
            // 現在のチェック状態を取得
            bool isChecked = EditorPrefs.GetBool(LOCK_PATH, true);

            // 状態を反転して保存
            EditorPrefs.SetBool(LOCK_PATH, !isChecked);
        }

        /// <summary>
        /// メニューのチェック表示を最新状態に更新する
        /// </summary>
        /// <returns>常に true（メニュー項目を有効にする）</returns>
        [MenuItem(LOCK_PATH, true)]
        private static bool ValidateLock()
        {
            // 最新のチェック状態を取得して、メニューのチェック表示を更新する
            bool isChecked = EditorPrefs.GetBool(LOCK_PATH, true);
            Menu.SetChecked(LOCK_PATH, isChecked);

            return true;
        }

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            SymphonyFileDontMove(movedAssets, movedFromAssetPaths);
        }

        /// <summary>
        /// SymphonyFrameWorkフォルダ内の物が移動されたら戻す
        /// </summary>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        private static void SymphonyFileDontMove(string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (int i = 0; i < movedAssets.Length; i++)
            {
                string oldPath = movedFromAssetPaths[i];
                string newPath = movedAssets[i];

                // 移動がSymphonyFrameWorkのアセットかどうかを判定
                if (oldPath.StartsWith(SymphonyConstant.FRAMEWORK_PATH))
                {
                    bool isLock = EditorPrefs.GetBool(LOCK_PATH, true);

                    //ロックされている時は移動できない
                    if (isLock)
                    {
                        if (EditorUtility.DisplayDialog(
                            "移動禁止",
                            $"SymphonyFrameWorkは移動できません\npath : '{oldPath}'",
                            "OK"))
                        {
                            // 移動を元に戻す
                            AssetDatabase.MoveAsset(newPath, oldPath);
                            AssetDatabase.Refresh();
                        }
                    }
                    //ロックされていない時は警告を出す
                    else
                    {
                        if (!EditorUtility.DisplayDialog(
                            "移動注意",
                            $"SymphonyFrameWorkを移動しようとしています。\n本当に移動しますか？\npath : '{oldPath}'",
                            "OK", "Cancel"))
                        {
                            // 移動を元に戻す
                            AssetDatabase.MoveAsset(newPath, oldPath);
                            AssetDatabase.Refresh();
                        }
                    }
                }
            }
        }
    }
}

