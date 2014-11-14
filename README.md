OctopusDeploy.Powershell
========================

A powershell module for automating octopus deploy functionality

The cmdlets provided in this module are simply wrappers over the official Octopus Deploy REST API. This module was originally created to fullfil some missing functionality from Octo.exe. Besides, powershell cmdlets provide a much nicer way to automate the CI processes.

Examples
========================

```
# Set the default octopus configuration`
Set-OctoDefaults -BaseUri <your octopus server url> -ApiKey <your api key>

# Get the environment with the specified name
$env = Get-OctoEnvironment -Name "MyEnvironment"

# Create a release in the specified project
$release = Get-OctoProject -Name "MyProject" | New-OctoRelease -Version 1.1.0

# Deploy the release in the specified environment and wait fo the deployment task to complete
$release | New-OctoDeployment -Environment $env | Wait-OctoTask
```

Terms
========================

This library is provided 'as-is' and Dimension Data cannot provide support for its usage.
