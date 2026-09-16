using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FindUnusedFBX
{
    [MenuItem("Tools/Buscar FBX no usados")]
    public static void Find()
    {
        string[] allAssets = AssetDatabase.GetAllAssetPaths();

        HashSet<string> usedFBX = new HashSet<string>();

        foreach (string asset in allAssets)
        {
            if (!asset.StartsWith("Assets/"))
                continue;

            if (asset.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                continue;

            if (asset.EndsWith(".meta", System.StringComparison.OrdinalIgnoreCase))
                continue;

            string[] dependencies = AssetDatabase.GetDependencies(asset, true);

            foreach (string dependency in dependencies)
            {
                if (dependency.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                {
                    usedFBX.Add(dependency);
                }
            }
        }

        List<string> unusedFBX = new List<string>();

        foreach (string asset in allAssets)
        {
            if (!asset.StartsWith("Assets/"))
                continue;

            if (!asset.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                continue;

            if (!usedFBX.Contains(asset))
            {
                unusedFBX.Add(asset);
            }
        }

        Debug.Log("==============================================");
        Debug.Log("           FBX NO USADOS");
        Debug.Log("==============================================");
        Debug.Log($"Total FBX encontrados: {CountFBX(allAssets)}");
        Debug.Log($"FBX usados: {usedFBX.Count}");
        Debug.Log($"FBX aparentemente NO usados: {unusedFBX.Count}");
        Debug.Log("==============================================");

        foreach (string fbx in unusedFBX)
        {
            long size = new FileInfo(fbx).Length;

            Debug.Log(
                $"NO USADO | {size / (1024f * 1024f):F2} MB | {fbx}"
            );
        }

        Debug.Log("==============================================");
        Debug.Log("ANALISIS TERMINADO");
        Debug.Log("==============================================");
    }

    private static int CountFBX(string[] assets)
    {
        int count = 0;

        foreach (string asset in assets)
        {
            if (asset.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                count++;
        }

        return count;
    }
}