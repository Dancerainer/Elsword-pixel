using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ElswordPixel.Editor
{
    public static class DemoBuilder
    {
        const string ScenePath = "Assets/Scenes/TreeOfEl.unity";
        const string ApkPath = "Builds/Android/Elsword-Pixel-1.0.2.apk";

        [MenuItem("Elsword Pixel/Build Android APK")]
        public static void BuildAndroid()
        {
            if (!File.Exists(ScenePath) || !AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Pixel/runtime-catalog.json"))
                throw new BuildFailedException("The release scene and catalog are required.");

            PlayerSettings.productName = "Elsword Pixel Ruben and Elder";
            PlayerSettings.bundleVersion = "1.0.2";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.elswordpixel.rubenelder");
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetIl2CppCompilerConfiguration(NamedBuildTarget.Android, Il2CppCompilerConfiguration.Master);
            PlayerSettings.Android.bundleVersionCode = 3;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7 | AndroidArchitecture.X86_64;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            Directory.CreateDirectory(Path.GetDirectoryName(ApkPath));
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = new[] { ScenePath },
                target = BuildTarget.Android,
                locationPathName = ApkPath,
                options = BuildOptions.CompressWithLz4HC
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException("Android build failed: " + report.summary.result);
        }
    }
}
