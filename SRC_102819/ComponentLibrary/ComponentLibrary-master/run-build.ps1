param(
	[Int32]$buildNumber=0
)
cls

Remove-Module [P]sake

$psakemodule = (Get-ChildItem ("Packages\psake*\tools\psake.psm1")).FullName | Sort-Object $_ | select -last 1
Import-Module $psakemodule

Invoke-psake -buildFile .\BuildAutomation\build.ps1 `
			 -taskList pack `
			 -properties @{
				 "buildConfiguration" = "Release"
				 "buildPlatform" = "Any CPU"} `
			 -parameters @{
				 "solutionFile" = "..\ComponentLibrary.sln"
				 "buildNumber" = $buildNumber
			 }
exit $LASTEXITCODE


