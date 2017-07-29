[CmdletBinding()]
param(
    [string] $targetPath,
    [string] $ssisVersion,
    [switch] $isRuntime = $false,
    [switch] $uninstall = $false
)

# Configurable per-system
$ssisBasePath = "C:\Program Files (x86)\Microsoft SQL Server"

function Load-Assembly {
    param(
        [parameter(Mandatory=$true)][string] $assemblyName
    )

    if (([AppDomain]::CurrentDomain.GetAssemblies() | where { $_ -match $assemblyName }) -eq $null)
    {
        [System.Reflection.Assembly]::LoadWithPartialName($assemblyName) > $null
    };
}

function Install-Gac {
    Param(
        [parameter(Mandatory=$true)][string] $assembly
    )    

    if ($PSCmdlet.ShouldProcess($assembly, "Installing to GAC")) {
        Write-Host "   Installing assembly '$assembly' into GAC"    
        Add-GacAssembly $assembly
    }
}

function Uninstall-Gac {
    Param(
        [parameter(Mandatory=$true)][string] $assembly
    )

    $assemblyName = [System.IO.Path]::GetFileNameWithoutExtension($assembly)

    # If it is in the GAC
	$gacAssembly = Get-GacAssembly -Name $assemblyName

	if ($gacAssembly) {
		if ($PSCmdlet.ShouldProcess($assemblyName, "Removing from GAC")) {
			Write-Host "    Removing assembly '$assemblyName' from GAC"
			Remove-GacAssembly -assemblyName $assemblyName
		}
	}
}

function Verify-Administrator {
    $principal = New-Object -TypeName "Security.Principal.WindowsPrincipal" -ArgumentList ([Security.Principal.WindowsIdentity]::GetCurrent())
    if (-not ($principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)))
    {
        throw "You must be an administrator to run this script"
    } else {
        Write-Debug "   Admin privileges detected"
    }
}

function Verify-Gac {
    if (Get-Module -ListAvailable -Name Gac) {
        Write-Debug "   GAC module found, importing"
    } else {
        Write-Debug "   GAC module not found, installing"
        Install-Module Gac
    }

    Import-Module Gac    
}

function Verify-PowershellVersion {
    $str = $PSVersionTable.PSVersion    
    $version = New-Object -TypeName "Version"
    if (-not [System.Version]::TryParse($str, [ref] $version) -or $version.Major -lt 5) {
        throw "This script requires Powershell v5 or higher"
    } else {
        Write-Debug "   Powershell version = $version"
    }
}

function Install-Files {            
    Write-Host "   Copying file to '$ssisPath'"    

    # Install into GAC
    Install-Gac $targetPath
    
    if ($isRuntime) {
        # Copy to designer
        if ($PSCmdlet.ShouldProcess($file, "Copying design-time file")) {
            Copy-Item -Path $targetPath -Destination $ssisPath -Force
        }
    }        
}

function Uninstall-Files {    

    # Remove from GAC
    Uninstall-Gac $targetPath
    
    if ($isRuntime) {
        $fileName = [System.IO.Path]::Combine($ssisPath, [System.IO.Path]::GetFileName($targetPath))

        # Remove from designer
        if ($PSCmdLet.ShouldProcess($fileName, "Removing design-time files")) {
            Remove-Item -Path $fileName -Force
        }
    }   
}

#################################################

# Verify admin rights
Write-Debug "Verifying system"
Verify-PowershellVersion
Verify-Administrator
Verify-Gac

# Validate the arguments
if (-not (Test-Path "$targetPath")) 
{ 
	#For uninstall we don't need to do anything else
	if ($uninstall) { return }

	throw "Target path not found at '$targetPath'" 
}
$targetFileName = [System.IO.Path]::GetFileName($targetPath)

$ssisPath = [System.IO.Path]::Combine($ssisBasePath, $ssisVersion + "\DTS\Tasks")
if (-not (Test-Path "$ssisPath")) { throw "SSIS Path not found at '$ssisPath'" }

# Process the files
if ($uninstall) {
    Uninstall-Files
} else {
    Install-Files
}








