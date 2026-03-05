using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System;
using System.IO;

public class AutoVersionOnBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;
    const string PrefKey = "GGTOOLS_BUILD_ENABLE_FEATURE_X";
    bool changeProductName = false;


    string version;

    public void OnPreprocessBuild(BuildReport report)
    {
        bool current = EditorPrefs.GetBool(PrefKey, false);

        changeProductName = !EditorUtility.DisplayDialog(
            "GGTools - Change Product Name",
            $"Plataform: {report.summary.platform}\n" +
            $"Do you want to keep the Product Name before starting the build?",
            "Yes",
            "No"
        );


        version = DateTime.Now.ToString("yyyyMMdd_HHmm");
        PlayerSettings.bundleVersion = version;
        if (changeProductName) 
        {
            PlayerSettings.productName = $"{version}_{PlayerSettings.productName}";
        }
        PlayerSettings.companyName = "Gaz Games";
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        

        var buildPath = report.summary.outputPath;

        var buildFolder = Path.GetDirectoryName(buildPath);

        var parent = Directory.GetParent(buildFolder).FullName;

        string projectName = GetBaseProjectName();

        string newFolder =
            Path.Combine(parent, $"{version}_{projectName}");

        if (!Directory.Exists(newFolder))
        {
            Directory.Move(buildFolder, newFolder);
        }

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
