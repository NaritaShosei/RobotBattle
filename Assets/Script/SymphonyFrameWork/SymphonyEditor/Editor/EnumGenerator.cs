﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SymphonyFrameWork.Core;
using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    public static class EnumGenerator
    {
        private static readonly Regex IdentifierRegex = new(@"^@?[a-zA-Z_][a-zA-Z0-9_]*$");
        private static readonly string[] ReservedWords = { "abstract", "as", "base", "bool", "break", "while" };

        public static async void EnumGenerate(string[] strings, string fileName)
        {
            //重複を削除
            var hash = new HashSet<string>(strings)
                .Where(s =>
                {
                    //文字列の頭文字がアルファベットではないものは除外
                    if (!IdentifierRegex.IsMatch(s))
                    {
                        Debug.LogWarning($"無効な文字で始まっているか無効な文字が含まれているため'{s}'を除外しました");
                        return false;
                    }

                    //プログラム文字を除外
                    if (ReservedWords.Contains(s)) Debug.LogWarning($"無効な文字列'{s}'を除外しました");

                    return true;
                })
                .ToHashSet();

            if (hash.Count <= 0) return;

            //ディレクトリを生成
            CreateResourcesFolder($"{SymphonyConstant.ENUM_PATH}/");

            //ファイル名を生成
            var enumFilePath = $"{SymphonyConstant.ENUM_PATH}/{fileName}Enum.cs";

            var content = NormalEnumGenerate(fileName, hash);

            await File.WriteAllLinesAsync(enumFilePath, content, Encoding.UTF8);
            File.SetLastAccessTime(enumFilePath, DateTime.Now);
            AssetDatabase.ImportAsset(enumFilePath, ImportAssetOptions.ForceUpdate);
            
            Debug.Log($"{fileName}Enumを生成しました");
        }

        /// <summary>
        ///     リソースフォルダが無ければ生成
        /// </summary>
        private static void CreateResourcesFolder(string resourcesPath)
        {
            //リソースがなければ生成
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
                AssetDatabase.ImportAsset(resourcesPath, ImportAssetOptions.ForceUpdate);
            }
        }

        /// <summary>
        ///     通常のEnumを生成する
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        private static IEnumerable<string> NormalEnumGenerate(string fileName, HashSet<string> hash)
        {
            //ファイルの中身を生成
            IEnumerable<string> content = new[] { "public enum " + fileName + "Enum : int\n{\n    None = 0," };

            //Enumファイルに要素を追加していく
            content = content.Concat(hash.Select((s, i) => $"    {s} = {i + 1},"));
            content = content.Append("}");

            return content;
        }

        private static IEnumerable<string> FlagEnumGenerate(string fileName, HashSet<string> hash)
        {
            //ファイルの中身を生成
            IEnumerable<string> content = new[]
                { "using System;\n\n[Flags]\npublic enum " + fileName + "Enum : int\n{\n    None = 0," };

            //Enumファイルに要素を追加していく
            content = content.Concat(hash.Select((s, i) => $"    {s} = 1 << {i + 1},"));
            content = content.Append("}");

            return content;
        }
    }
}