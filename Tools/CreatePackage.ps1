# $msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
$msbuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"

.\IncrementVersion-cs.ps1 ..\Bellona\Analysis

$slnFilePath = "..\Bellona\Bellona.sln"
& $msbuild $slnFilePath /p:Configuration=Release /t:Clean
& $msbuild $slnFilePath /p:Configuration=Release /t:Rebuild

cd ..\Bellona\Analysis
.\NuGetPackup.exe

move *.nupkg ..\..\Published -Force
explorer ..\..\Published
