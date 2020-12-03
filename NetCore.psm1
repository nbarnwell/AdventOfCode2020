
function New-NetCoreSolution {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory, ValueFromPipeline)]
        [string[]] $Name
    )
    process {
        $Name |
            ForEach-Object {
                dotnet new sln --name $_ | Write-Host -ForegroundColor Magenta
            }
    }
}

function New-NetCoreProject {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string] $Name,
        [Parameter()]
        [string] $TemplateName = 'classlib'
    )
    process {
        dotnet new $TemplateName --name $Name | Write-Host -ForegroundColor Magenta
        Get-Item ".\\$Name\\$Name.csproj"
    }
}

function Add-NetCoreProjectsToSolution {
    [CmdletBinding()]
    param (
        [Parameter(ValueFromPipeline)]
        [string[]] $ProjectFile = (Get-ChildItem -r -inc *.csproj),
        [Parameter(Mandatory)]
        [string] $SolutionFile
    )
    process {
        $ProjectFile |
            ForEach-Object {
                dotnet sln add $_
            }
    }
}