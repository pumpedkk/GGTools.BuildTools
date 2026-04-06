using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System;
using System.IO;

namespace GGTools.BuildTools
{

    /// <summary>
    /// Automatically manages versioning and build folder organization during the Unity build process.
    /// 
    /// Before the build starts, it:
    /// - Generates a timestamp-based version.
    /// - Optionally modifies the product name with the version prefix.
    /// - Requests patch notes from the user and saves them alongside the build.
    /// 
    /// After the build completes, it:
    /// - Renames the generated build folder using the version and project name.
    /// - Restores the original product name.
    /// </summary>
    public class AutoVersionOnBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        const string COMPANY_NAME = "Gaz Games";

        public int callbackOrder => 0;
        const string PrefKey = "GGTOOLS_BUILD_ENABLE_FEATURE_X";
        bool changeProductName = false;
        string patchNotes;


        string version;

        /// <summary>
        /// Called automatically by Unity before the build starts.
        /// 
        /// Prompts the user to decide whether to keep the product name,
        /// requests patch notes, generates a timestamp-based version,
        /// and updates PlayerSettings accordingly.
        /// </summary>

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
            TextInputDialog.Show("Would you kindly leave a Patch Notes for this build?", "100 characters", (value) =>
            {
                patchNotes = value;
                var buildPath = report.summary.outputPath;

                var buildFolder = Path.GetDirectoryName(buildPath);

                File.WriteAllText(Path.Combine(buildFolder, "patchNotes.txt"), patchNotes);

            });



            version = DateTime.Now.ToString("yyyyMMdd_HHmm");
            PlayerSettings.bundleVersion = version;
            if (changeProductName)
            {
                PlayerSettings.productName = $"{version}_{PlayerSettings.productName}";
            }
            PlayerSettings.companyName = COMPANY_NAME;
        }

        /// <summary>
        /// Called automatically by Unity after the build finishes.
        /// 
        /// Renames the generated build folder to include the version and
        /// restores the original product name used by the project.
        /// </summary>

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

        /// <summary>
        /// Retrieves the original project name by removing a version prefix
        /// if the product name already contains a timestamp-based version.
        /// </summary>
        /// <returns>The base project name without the version prefix.</returns>
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

        /// <summary>
        /// Called automatically by Unity when the package is imported.
        /// 
        /// Auto setup some informations that are shared across all projects.
        /// </summary>
        [InitializeOnLoadMethod]
        public static void InitialProjectSetup()
        {
            const string flagPath = "UserSettings/BuildTools.InitialSetup";
            if (File.Exists(flagPath)) return;
            File.WriteAllText(flagPath, "1");

            PlayerSettings.companyName = COMPANY_NAME;
            PlayerSettings.Android.textureCompressionFormats = new TextureCompressionFormat[]{ TextureCompressionFormat.ASTC};

            string applicationIdentifier = $"com.{PlayerSettings.companyName.Replace(" ", String.Empty)}.{PlayerSettings.productName.Replace(" ", String.Empty)}";
            #if UNITY_6000_0_OR_NEWER
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, applicationIdentifier);
            #else
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, applicationIdentifier);
            #endif



            AssetDatabase.SaveAssets();
        }
    }
}