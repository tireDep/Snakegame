using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WebGLBuild
{
    // Build Settings에서 활성화된 씬을 GitHub Pages용 WebGL 플레이어로 빌드합니다.
    public static void Build()
    {
        string projectRoot = Directory.GetParent(Application.dataPath)?.FullName
            ?? throw new InvalidOperationException("프로젝트 루트 경로를 찾을 수 없습니다."); // Unity 프로젝트 루트 경로
        string outputPath = Path.Combine(projectRoot, "Builds", "WebGL"); // WebGL 빌드 출력 경로
        string[] enabledScenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray(); // Build Settings에서 활성화된 씬 경로 목록

        // 배포할 씬이 없으면 빈 플레이어가 만들어지지 않도록 빌드를 중단합니다.
        if (enabledScenes.Length == 0)
        {
            throw new BuildFailedException("Build Settings에 활성화된 씬이 없습니다.");
        }

        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = enabledScenes,
            locationPathName = outputPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        }; // WebGL 플레이어 빌드 옵션

        // Unity 빌드 파이프라인을 실행하고 결과를 검증합니다.
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions); // Unity 빌드 결과 보고서
        BuildSummary summary = report.summary; // 성공 여부와 출력 크기를 담은 빌드 요약

        if (summary.result != BuildResult.Succeeded)
        {
            throw new BuildFailedException($"WebGL 빌드 실패: {summary.result}");
        }

        Debug.Log($"WebGL 빌드 성공: {outputPath} ({summary.totalSize} bytes)");
    }
}
