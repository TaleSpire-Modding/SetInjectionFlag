$path = "SetInjectionFlag"
$json = Get-Content "../manifest.json" | ConvertFrom-Json
$version = $json.version_number
(Get-Content SetInjectionFlag.cs) -Replace $version,"0.0.0" | Set-Content SetInjectionFlag.cs
(Get-Content SetInjectionFlag.csproj) -Replace $version,"0.0.0" | Set-Content SetInjectionFlag.csproj