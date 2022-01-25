Function check-result {
	if ($LastExitCode -ne 0) {
		$e = [char]27
		$start = "$e[1;31m"
		$end = "$e[m"
		$text = "ERROR: Exiting with error code $LastExitCode"
		Write-Host "$start$text$end"
		return $false
	}
	return $true
}

Function Invoke-Exe {
Param(
    [parameter(Mandatory=$true)][string] $cmd,
    [parameter(Mandatory=$true)][string] $args
)

	Write-Host "Executing: `"$cmd`" --% $args"
	Invoke-Expression "& `"$cmd`" $args"
	$result = check-result
	if (!$result) {
		throw "ERROR executing EXE"
	}
}

#$ErrorActionPreference = 'Stop'

echo "build: Build started"
$version = @{ $true = $env:APPVEYOR_BUILD_VERSION; $false = "1.0.0" }[$env:APPVEYOR_BUILD_VERSION -ne $NULL];

# build
$args = "clean $PSScriptRoot\src"
Invoke-Exe -cmd dotnet -args $args
$args = "build src --configuration Debug /property:Version=$version"
Invoke-Exe -cmd dotnet -args $args

gci -filter *tests.csproj -recurse | %{ dotnet test $_.FullName --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover }
