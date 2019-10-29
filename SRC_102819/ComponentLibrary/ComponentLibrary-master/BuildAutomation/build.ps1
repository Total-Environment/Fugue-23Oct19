Include ".\helpers.ps1"

properties {
	$solutionDirectory = (Get-Item $solutionFile).DirectoryName
	$outputDirectory = "$solutionDirectory\.build"
	$tempOutputDirectory = "$outputDirectory\Temp"
	$testResultsDirectory = "$outputDirectory\TestResults"
	$buildConfiguration = "Debug"
	$buildPlatform = "x64"
	$publishedXUnitDirectory = "$tempoutputDirectory\_publishedXunitTests"
	$xUnitTestResultsDirectory = "$testResultsDirectory\xUnit"
	$testCoverageDirectory = "$outputDirectory\TestCoverage"
	$testCoverageReportPath = "$testCoverageDirectory\OpenCover.xml"
	$packagesPath = "$solutionDirectory\packages"
	$xUnitExe = (Find-PackagePath $packagesPath "xunit.runner.console" ) + "\Tools\xunit.console.exe"
	$openCoverExe = (Find-PackagePath $packagesPath "OpenCover" ) + "\Tools\OpenCover.console.exe"
	$reportGeneratorExe = (Find-PackagePath $packagesPath "ReportGenerator" ) + "\Tools\ReportGenerator.exe"
	$nugetExe = (Find-PackagePath $packagesPath "Nuget.CommandLine" ) + "\Tools\Nuget.exe"
	$testCoverageFilter = """""+[ComponentLibrary]* +[ExcelImporter]* -[ComponentLibrary]*Dto.* -[ComponentLibrary]*Dao.*"""""
	$testCoverageExcludeByAttribute = "System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute"
	$testCoverageExcludeByFile = "`"*\*Designer.cs;*\*.g.cs;*\*.g.i.cs`""
	$publishedWebsitesDirectory = "$tempOutputDirectory\_PublishedWebsites"
	$packagesDirectory = "$outputDirectory\Packages"
	$msBuildexe = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
}

FormatTaskName "`r`n`r`n---------- Executing {0} task -------------------"

task default -depends test

# Verify dependencies, setting up folders, and other initialization tasks
task init -description "Initialises build by removing privious artifacts and create output directories" -requiredVariables outputDirectory, tempOutputDirectory {

	Assert -conditionToCheck ("Debug", "Release" -contains $buildConfiguration) `
		   -failureMessage "Invalid build configuration $buildConfiguration. Valid values are 'Debug' and 'Release'"
	Assert -conditionToCheck ("X86", "X64", "Any CPU" -contains $buildPlatform) `
		   -failureMessage "Inalid build platform $buildPlatform. Valid values are 'X86', 'X64', 'Any CPU'"
	Assert(Test-Path $xUnitExe) "Xunit console could not be found."
	Assert(Test-Path $openCoverExe) "Open Cover console could not be found."
	Assert(Test-Path $reportGeneratorExe) "Report generator could not be found."
	Assert(Test-Path $nugetExe) "Nuget could not be found."

	if(Test-Path $outputDirectory) {
		Write-Host "Deleting $outputDirectory"
		Remove-Item $outputDirectory -Force -Recurse
	}

	

	Write-Host "Create a output folder located at $outputDirectory"
	New-Item $outputDirectory -ItemType Directory | Out-Null

	Write-Host "Create a temp folder located at $tempoutputDirectory"
	New-Item $tempOutputDirectory -ItemType Directory | Out-Null

	Write-Host "Create a test results folder located at $testResultsDirectory"
	New-Item $testResultsDirectory -ItemType Directory | Out-Null

	Write-Host "Create a test coverate folder located at $testCoverageDirectory"
	New-Item $testCoverageDirectory -ItemType Directory | Out-Null

	Write-Host "Create a packages folder located at $packagesDirectory"
	New-Item $packagesDirectory -ItemType Directory | Out-Null
}

task test -depends init, compile, integration-test {
	Write-Host "Testing"

	if(Test-Path $testCoverageReportPath) {
		Write-Host "`r`nGenerating HTML test coverage report"
		exec {&$reportGeneratorExe $testCoverageReportPath $testCoverageDirectory}
	}
	else {
		Write-Host "No coverage file found at: $testCoverageReportPath"
	}

	Write-Host "Parsing opencover results"
	$coverage = [xml] (Get-Content -Path $testCoverageReportPath)

	$coverageSummary = $coverage.CoverageSession.Summary

	$numClasses = $coverageSummary.numClasses
	if($numClasses -eq 0){
		$numClasses = 0.00000001
	}

	$numMethods = $coverageSummary.numMethods
	if($numMethods -eq 0){
		$numMethods = 0.00000001
	}

	$numBranchPoints = $coverageSummary.numBranchPoints
	if($numBranchPoints -eq 0){
		$numBranchPoints = 0.00000001
	}

	# class metrics
	Write-Host "##teamcity[buildStatisticValue key='CodeCoverageAbsCCovered' value='$($coverageSummary.visitedClasses)']"
	Write-Host "##teamcity[buildStatisticValue key='CodeCoverageAbsCTotal' value='$($coverageSummary.numClasses)']"
	Write-Host ("##teamcity[buildStatisticValue key='CodeCoverageC' value='{0:N2}']" -f (($coverageSummary.visitedClasses/$numClasses)*100))

	#method metrics
	Write-Host "##teamcity[buildStatisticValue key='CodeCoverageAbsMCovered' value='$($coverageSummary.visitedMethods)']"
	Write-Host "##teamcity[buildStatisticValue key='CodeCoverageAbsMTotal' value='$($coverageSummary.numMethods)']"
	Write-Host ("##teamcity[buildStatisticValue key='CodeCoverageM' value='{0:N2}']" -f (($coverageSummary.visitedMedthods/$numMethods)*100))

	#branch metrics
	Write-Host "##teamcity[buildStatisticValue key='CodeCoverageAbsBCovered' value='$($coverageSummary.visitedBranchPoints)']"
	Write-Host "##teamcity[buildStatisticValue key='CodeCoverageAbsBTotal' value='$($coverageSummary.numBranchPoints)']"
	Write-Host ("##teamcity[buildStatisticValue key='CodeCoverageB' value='{0:N2}']" -f (($coverageSummary.visitedBranchPoints/$numBranchPoints)*100))

	#statement metrics
	Write-Host "##teamcity[buildStatisticValue key='CodeCoverageAbsSCovered' value='$($coverageSummary.visitedSequencePoints)']"
	Write-Host "##teamcity[buildStatisticValue key='CodeCoverageAbsSTotal' value='$($coverageSummary.numSequencePoints)']"
	Write-Host "##teamcity[buildStatisticValue key='CodeCoverageS' value='$($coverageSummary.sequenceCoverage)']"
}

task integration-test -depends init, compile `
					  -description "Run Integration Tests" `
					  -precondition {return Test-Path $publishedXunitDirectory} {

						  Write-Host $publishedXunitTestDirectory
						  $projects = Get-ChildItem $publishedXUnitDirectory
						  if($projects.Count -eq 1) {
							  Write-Host "1 XUnit Project has been found:"
						  }
						  else {
							  Write-Host $projects.Count " XUnit projects have been found:"
						  }

						  Write-Host ($projects | select $_.Name)

						  if(!(Test-Path $xUnitTestResultsDirectory)){
							  Write-Host "Created test results directory at $xUnitTestResultsDirectory"
							  mkdir $xUnitTestResultsDirectory | Out-Null
						  }

						  $testAssemblies = $projects | ForEach-Object { "`"`"" + $_.FullName + "\" + $_.Name + ".dll`"`"" }

						  $testAssembliesParameters = [String]::Join(" ", $testAssemblies)

						  $targetArgs = "$testAssembliesParameters -xml `"`"$xUnitTestResultsDirectory\XUnit.xml`"`" -nologo -noshadow"

						 exec {&$openCoverExe -target:$xUnitExe `
											   -targetargs:$targetArgs `
											   -output:$testCoverageReportPath `
											   -register:user `
											   -filter:$testCoverageFilter `
											   -excludeByAttribute:$testCoverageExcludeByAttribute `
											   -excludeByFile:$testCoverageExcludeByFile `
											   -skipautoprops `
											   -hideskipped:All `
											   -returntargetcode}
}

task pack -depends init, compile, test -requiredVariables publishedWebsitesDirectory, packagesDirectory    {

	$applications = @(Get-ChildItem $publishedWebsitesDirectory)

	foreach($application in $applications) {
		$nuspecPath = $application.FullName + "\" + $application.Name + ".nuspec"
		$nuspec = [xml] (Get-Content -Path $nuspecPath)

		$metadata = $nuspec.package.metadata
		$metadata.version = $metadata.version.Replace("[buildNumber]", $buildNumber)
		$nuspec.Save((Get-Item $nuspecPath))

		$nuspec = [xml] (Get-Content -Path $nuspecPath)
		Write-Host "nuget version:" $nuspec.package.metadata.version

		exec {&$nugetExe pack $nuspecPath -outputDirectory $packagesDirectory}
	}
	
}

task compile -depends init {
	Write-Host "Compiling code!!"
	exec {&$msBuildexe $solutionFile "/p:Configuration=$buildConfiguration;Platform=$buildPlatform;OutDir=$tempOutputDirectory"}
}