$path = "SetInjectionFlag"
$json = Get-Content "../manifest.json" | ConvertFrom-Json
$version = $json.version_number
(Get-Content SetInjectionFlag.cs) -Replace "0.0.0", $version | Set-Content SetInjectionFlag.cs
(Get-Content SetInjectionFlag.csproj) -Replace "0.0.0", $version | Set-Content SetInjectionFlag.csproj