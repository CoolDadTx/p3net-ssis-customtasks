[CmdletBinding()]
param(
    [string] $ssisVersion = "130",
    [switch] $includeDesignerFiles = $true
)

# Get the file needed by the runtime and design time
$runtimeFiles = @()
$designFiles = @()

$files = Get-ChildItem . -Filter "P3Net.IntegrationServices*.dll" -File
if ($files.Length -eq 0) {
    throw "No files found"
}

foreach ($file in $files) 
{
    if ($file.Name -like "*.UI.dll") { $designFiles += $file }
    else { $runtimeFiles += $file }      
}

# Install the runtime files
foreach ($file in $runtimeFiles) 
{
	& .\RegisterAssembly.ps1 -targetPath $file -ssisVersion $ssisVersion -isRuntime
}

# Optionally install the designer files
if ($includeDesignerFiles)
{
	foreach ($file in $designFiles) 
	{
		& .\RegisterAssembly.ps1 -targetPath $file -ssisVersion $ssisVersion
	}
}
