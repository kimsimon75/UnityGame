// Assets/Editor/ScriptLineStats.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text;

public static class ScriptLineStats
{
    // 여기 폴더만 바꾸면 됨:
    // 예) "Assets/스크립트" / "Assets/Scripts" / "Assets/MyFolder/Scripts"
    private static readonly string TargetFolder = "Assets/Scenes/Script";

    [MenuItem("Tools/Script Line Stats")]
    public static void Run()
    {
        if (!AssetDatabase.IsValidFolder(TargetFolder))
        {
            Debug.LogError($"폴더가 없어요: {TargetFolder}\nTargetFolder 값을 실제 폴더로 바꿔줘.");
            return;
        }

        var guids = AssetDatabase.FindAssets("t:Script", new[] { TargetFolder });
        var paths = guids.Select(AssetDatabase.GUIDToAssetPath)
                         .Where(p => p.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                         .Distinct()
                         .OrderBy(p => p)
                         .ToArray();

        if (paths.Length == 0)
        {
            Debug.LogWarning($"대상 폴더에서 .cs 스크립트를 못 찾았어요: {TargetFolder}");
            return;
        }

        int totalLines = 0;
        int totalNonEmptyLines = 0;
        int globalMaxLineLen = 0;
        string globalMaxLineFile = "";
        int globalMaxLineNo = 0;

        var sb = new StringBuilder();
        sb.AppendLine("path,lines,nonEmptyLines,maxLineLen,maxLineNo");

        foreach (var assetPath in paths)
        {
            var fullPath = Path.GetFullPath(assetPath);

            int lines = 0;
            int nonEmpty = 0;
            int maxLen = 0;
            int maxNo = 0;

            // 스트리밍 방식: 큰 파일에도 안전
            int lineNo = 0;
            foreach (var line in File.ReadLines(fullPath, Encoding.UTF8))
            {
                lineNo++;
                lines++;
                if (!string.IsNullOrWhiteSpace(line)) nonEmpty++;

                int len = line.Length;
                if (len > maxLen)
                {
                    maxLen = len;
                    maxNo = lineNo;
                }
            }

            totalLines += lines;
            totalNonEmptyLines += nonEmpty;

            if (maxLen > globalMaxLineLen)
            {
                globalMaxLineLen = maxLen;
                globalMaxLineFile = assetPath;
                globalMaxLineNo = maxNo;
            }

            sb.AppendLine($"{EscapeCsv(assetPath)},{lines},{nonEmpty},{maxLen},{maxNo}");
            Debug.Log($"[ScriptLineStats] {assetPath}\n  lines={lines}, nonEmpty={nonEmpty}, maxLineLen={maxLen} (line {maxNo})");
        }

        Debug.Log(
            $"[ScriptLineStats] DONE\n" +
            $"- folder: {TargetFolder}\n" +
            $"- files: {paths.Length}\n" +
            $"- total lines: {totalLines}\n" +
            $"- total non-empty lines: {totalNonEmptyLines}\n" +
            $"- global max line length: {globalMaxLineLen} chars @ {globalMaxLineFile}:{globalMaxLineNo}"
        );

        // CSV 저장 (프로젝트 루트)
        var outPath = Path.Combine(Directory.GetParent(Application.dataPath)!.FullName, "ScriptLineStats.csv");
        File.WriteAllText(outPath, sb.ToString(), Encoding.UTF8);
        Debug.Log($"[ScriptLineStats] CSV saved: {outPath}");
    }

    private static string EscapeCsv(string s)
    {
        if (s.Contains(",") || s.Contains("\""))
            return $"\"{s.Replace("\"", "\"\"")}\"";
        return s;
    }
}
#endif
