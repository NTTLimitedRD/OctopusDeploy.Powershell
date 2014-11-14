If (-not (Test-Path Env:\TF_BUILD_BUILDURI))
{
    Write-Host 'TF_BUILD_BUILDURI environment variable is not defined.';

    Return;
}

$buildUri = [System.Uri](Get-Content Env:\TF_BUILD_BUILDURI)
$buildId = Split-Path -Leaf $buildUri.LocalPath

Function UpdateAssemblyInfoWithBuildNumber([string] $solutionAssemblyInfoFile)
{
    $solutionAssemblyInfo = Get-Content $solutionAssemblyInfoFile
    $updatedVersionInfo = $solutionAssemblyInfo -replace "Version\(`"(\d+)\.(\d+)\.(\d+)\.\d+`"\)\]`$", "Version(`"`$1`.`$2`.$buildId.0`")]"

    $updatedVersionInfo | Out-File $solutionAssemblyInfoFile
}

Function UpdateMergeModuleScriptWithBuildNumber([string] $wixScriptFile)
{
    $wixScript = New-Object -TypeName 'System.Xml.XmlDocument'
    $wixScript.PreserveWhitespace = $true
    $wixScript.Load($wixScriptFile)

    $wixScript.Wix.Module.Version = $wixScript.Wix.Module.Version -replace "(\d+)\.(\d+)\.(\d+)", "`$1`.`$2`.$buildId"

    $wixScript.Save($wixScriptFile)
}

Function UpdateSetupScriptWithBuildNumber([string] $wixScriptFile)
{
    $wixScript = New-Object -TypeName 'System.Xml.XmlDocument'
    $wixScript.PreserveWhitespace = $true
    $wixScript.Load($wixScriptFile)

    $wixScript.Wix.Product.Version = $wixScript.Wix.Product.Version -replace "(\d+)\.(\d+)\.(\d+)", "`$1`.`$2`.$buildId"

    $wixScript.Save($wixScriptFile)
}

Function UpdateSqlProjectWithBuildNumber([string] $sqlProjectFile)
{
    $sqlProject = Get-Content $sqlProjectFile
    $sqlProject = $sqlProject -replace "\<DacVersion\>(\d+)\.(\d+)\.(\d+)\.(\d+)\<\/DacVersion\>", "<DacVersion>`$1`.`$2`.$buildId.`$4</DacVersion>"

    Set-Content $sqlProjectFile -Value $sqlProject
}

Function UpdateNuSpecWithBuildNumber([string] $nuSpecFile)
{
    $nuSpec = Get-Content $nuSpecFile
    $nuSpec = $nuSpec -replace "\<version\>(\d+)\.(\d+)\.(\d+).(\d+)\<\/version\>", "<version>1.1.$buildId.0</version>"

    Set-Content $nuSpecFile -Value $nuSpec
}

$currentDir = (Get-Location).Path
UpdateAssemblyInfoWithBuildNumber(Join-Path $currentDir "SolutionAssemblyInfo.cs")
UpdateNuSpecWithBuildNumber(Join-Path $currentDir "OctopusDeploy.Powershell\OctopusDeploy.Powershell.nuspec")
