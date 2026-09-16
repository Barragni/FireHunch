using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildReportAnalyzer
{
    [MenuItem("Tools/Analizar Build Report")]
    public static void Analyze()
    {
        BuildReport report = BuildReport.GetLatestReport();

        if (report == null)
        {
            Debug.LogError("No se encontró LastBuild.buildreport.");
            return;
        }

        Debug.Log("==============================================");
        Debug.Log("        ANALISIS DEL ULTIMO BUILD");
        Debug.Log("==============================================");

        Debug.Log(
            $"Tamaño total del build: " +
            $"{BytesToMB(report.summary.totalSize):F2} MB"
        );

        var assets = report.packedAssets
            .SelectMany(bundle => bundle.contents)
            .Where(asset => !string.IsNullOrEmpty(asset.sourceAssetPath))
            .OrderByDescending(asset => asset.packedSize)
            .Take(50)
            .ToList();

        Debug.Log("==============================================");
        Debug.Log("      50 ASSETS QUE MAS PESAN");
        Debug.Log("==============================================");

        for (int i = 0; i < assets.Count; i++)
        {
            var asset = assets[i];

            Debug.Log(
                $"{i + 1}. " +
                $"{BytesToMB(asset.packedSize):F2} MB | " +
                $"{asset.type} | " +
                $"{asset.sourceAssetPath}"
            );
        }

        Debug.Log("==============================================");
        Debug.Log("ANALISIS TERMINADO");
        Debug.Log("==============================================");
    }

    private static double BytesToMB(ulong bytes)
    {
        return bytes / (1024.0 * 1024.0);
    }
}