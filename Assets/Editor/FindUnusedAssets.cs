using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class FindUnusedAssets
{
    private static readonly string[] Extensions =
    {
        ".fbx",
        ".obj",
        ".png",
        ".jpg",
        ".jpeg",
        ".tga",
        ".psd",
        ".exr",
        ".wav",
        ".mp3",
        ".ogg",
        ".mat",
        ".anim",
        ".controller",
        ".prefab"
    };

    [MenuItem("Tools/Buscar Assets No Usados")]
    public static void Find()
    {
        string[] allAssets = AssetDatabase.GetAllAssetPaths();

        HashSet<string> referencedAssets = new HashSet<string>();

        foreach (string asset in allAssets)
        {
            if (!asset.StartsWith("Assets/"))
                continue;

            if (asset.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                continue;

            string[] dependencies = AssetDatabase.GetDependencies(asset, true);

            foreach (string dependency in dependencies)
            {
                // IMPORTANTE:
                // No contamos el propio asset como una referencia.
                if (dependency != asset)
                {
                    referencedAssets.Add(dependency);
                }
            }
        }

        List<(string path, long size)> unusedAssets =
            new List<(string path, long size)>();

        foreach (string asset in allAssets)
        {
            if (!asset.StartsWith("Assets/"))
                continue;

            if (!IsTargetExtension(asset))
                continue;

            if (referencedAssets.Contains(asset))
                continue;

            if (asset.Contains("/Resources/"))
                continue;

            if (asset.Contains("/StreamingAssets/"))
                continue;

            string fullPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                asset
            );

            if (!File.Exists(fullPath))
                continue;

            long size = new FileInfo(fullPath).Length;

            unusedAssets.Add((asset, size));
        }

        unusedAssets = unusedAssets
            .OrderByDescending(x => x.size)
            .ToList();

        Debug.Log("==============================================");
        Debug.Log("       ASSETS APARENTEMENTE NO USADOS");
        Debug.Log("==============================================");
        Debug.Log($"Total assets revisados: {allAssets.Length}");
        Debug.Log($"Candidatos encontrados: {unusedAssets.Count}");
        Debug.Log("==============================================");

        foreach (var asset in unusedAssets)
        {
            double mb = asset.size / (1024.0 * 1024.0);

            Debug.Log(
                $"NO USADO | {mb:F2} MB | {asset.path}"
            );
        }

        Debug.Log("==============================================");
        Debug.Log("ANALISIS TERMINADO");
        Debug.Log("==============================================");
    }

    private static bool IsTargetExtension(string path)
    {
        foreach (string extension in Extensions)
        {
            if (path.EndsWith(
                extension,
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}