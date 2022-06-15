[cmdletBinding()]
param(
	[switch]$update=[switch]::Present
)

# setup for coverage analysis
gci *Test*.csproj -Recurse | % { if (select-string -inputobject $_ -Pattern "coverlet.msbuild") { echo "remove coverlet.msbuild to $_.Fullname"; dotnet remove $_.FullName package coverlet.msbuild } }
gci *Test*.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern "coverlet.collector")){ echo "add coverlet.collector to $_.Fullname"; dotnet add $_.FullName package coverlet.collector }}

# analyzers
gci *.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern "Microsoft.VisualStudio.Threading.Analyzers")){ echo "add Microsoft.VisualStudio.Threading.Analyzers to $_.Fullname"; dotnet add $_.FullName package Microsoft.VisualStudio.Threading.Analyzers }}
gci *.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern "AsyncAnalyzers")){ echo "add AsyncAnalyzers to $_.Fullname"; dotnet add $_.FullName package AsyncAnalyzers }}


dotnet tool update --global dotnet-outdated-tool
dotnet outdated ./src --version-lock Major --pre-release Never --upgrade
