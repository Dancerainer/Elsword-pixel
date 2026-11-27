param(
    [ValidateSet('Android')][string]$Target = 'Android',
    [Parameter(Mandatory)][string]$EditorPath
)
$ErrorActionPreference = 'Stop'
$project = Split-Path -Parent $PSScriptRoot
$editor = (Resolve-Path -LiteralPath $EditorPath).Path
$reports = Join-Path $project 'Reports'
New-Item -ItemType Directory -Force -Path $reports | Out-Null
$log = Join-Path $reports 'editor-android.log'
$argsForUnity = @('-batchmode', '-quit', '-projectPath', "`"$project`"", '-buildTarget', $Target,
    '-executeMethod', 'ElswordPixel.Editor.DemoBuilder.BuildAndroid', '-logFile', "`"$log`"")
$unity = Start-Process -FilePath $editor -ArgumentList $argsForUnity -WindowStyle Hidden -PassThru
while (!$unity.WaitForExit(30000)) { Write-Output "Building Android APK. Log: $log" }
if ($unity.ExitCode -ne 0) { throw "Unity exited with code $($unity.ExitCode). See $log." }
$apk = Join-Path $project 'Builds/Android/Elsword-Pixel-1.0.2.apk'
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zip = [System.IO.Compression.ZipFile]::OpenRead($apk)
try {
    foreach ($name in @('assets/bin/Data/data.unity3d', 'assets/bin/Data/Managed/Metadata/global-metadata.dat', 'lib/arm64-v8a/libil2cpp.so')) {
        $entry = $zip.GetEntry($name)
        if ($null -eq $entry -or $entry.Length -eq 0) { throw "The APK is missing $name." }
    }
} finally {
    $zip.Dispose()
}
Write-Output $apk
