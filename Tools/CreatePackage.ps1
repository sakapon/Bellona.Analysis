$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

.\IncrementVersion-cs.ps1 ..\Bellona\Analysis

$slnFilePath = "..\Bellona\Bellona.sln"
Invoke-Expression ([string]::Format("{0} {1} /p:Configuration=Release /t:Clean", $msbuild, $slnFilePath))
Invoke-Expression ([string]::Format("{0} {1} /p:Configuration=Release /t:Rebuild", $msbuild, $slnFilePath))

cd ..\Bellona\Analysis
.\NuGetPackup.exe

move *.nupkg ..\..\Published -Force
explorer ..\..\Published
