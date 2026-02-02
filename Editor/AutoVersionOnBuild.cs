using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System;
using System.IO;

public class AutoVersionOnBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    string version;

    public void OnPreprocessBuild(BuildReport report)
    {
        string projectName = GetBaseProjectName();
        version = DateTime.Now.ToString("yyyyMMdd_HHmm");

        PlayerSettings.productName  = $"{version}_{projectName}";
        PlayerSettings.bundleVersion = version;
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        // pasta onde a build foi gerada
        var buildPath = report.summary.outputPath;

        // no Windows o outputPath vem como:
        // .../MinhaPasta/MeuExe.exe
        var buildFolder = Path.GetDirectoryName(buildPath);

        var parent = Directory.GetParent(buildFolder).FullName;

        string projectName = GetBaseProjectName();

        string newFolder =
            Path.Combine(parent, $"{version}_{projectName}");

        if (!Directory.Exists(newFolder))
        {
            Directory.Move(buildFolder, newFolder);
        }

        // restaura o nome do produto
        PlayerSettings.productName = projectName;
    }

    static string GetBaseProjectName()
    {
        string currentName = PlayerSettings.productName;

        if (System.Text.RegularExpressions.Regex.IsMatch(
            currentName,
            @"^\d{8}_\d{4}_"))
        {
            return currentName.Substring(14);
        }

        return currentName;
    }
}
