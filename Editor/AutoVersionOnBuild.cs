using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System;

public class AutoVersionOnBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string projectName = GetBaseProjectName();
        string version = DateTime.Now.ToString("yyyyMMdd_HHmm");

        PlayerSettings.productName  = $"{version}_{projectName}";
        PlayerSettings.bundleVersion = version;

    }
    public void OnPostprocessBuild(BuildReport report)
    {
        PlayerSettings.productName = GetBaseProjectName();
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
