﻿function Find-PackagePath {

	[CmdletBinding()]
	param(
		[Parameter(Position=0,Mandatory=1)]$packagesPath,
		[Parameter(position=1,Mandatory=1)]$packageName
	)
	return (Get-ChildItem ($packagesPath + "\" + $packageName + "*")).FullName | Sort-Object $_ | select -last 1
}