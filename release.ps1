New-Item -Path 'thunderstore' -ItemType Directory
Copy-Item -Path icon.png -Destination 'thunderstore'
Copy-Item -Path License.txt -Destination 'thunderstore'
Copy-Item -Path manifest.json -Destination 'thunderstore'
Copy-Item -Path README.md -Destination 'thunderstore'
Copy-Item -Path SetInjectionFlag/bin/Release/net48/SetInjectionFlagPlugin.dll -Destination 'thunderstore'
