# Set Injection Flag Plugin

This is a plugin for TaleSpire using BepInEx. This plugin lets BouncyRock developers know that logs being generated are from vanilla or modded instances.

## Install

Easiest way to install this is via R2Modman

## Usage

Just install, this will automatically update TaleSpire so the devs know you are doing modding work.

## How to Compile / Modify

Open ```SetInjectionFlagPlugin.sln``` in Visual Studio.

You will need to add references to:

```
* BepInEx.dll  (Download from the BepInEx project.)
* Bouncyrock.TaleSpire.Runtime (found in Steam\steamapps\common\TaleSpire\TaleSpire_Data\Managed)
* UnityEngine.dll
* UnityEngine.CoreModule.dll
* UnityEngine.InputLegacyModule.dll 
* UnityEngine.UI
* Unity.TextMeshPro
```

Build the project.

Browse to the newly created ```bin/Debug``` or ```bin/Release``` folders and copy the ```SetInjectionFlagPlugin.dll``` to ```Steam\steamapps\common\TaleSpire\BepInEx\plugins```

## Changelog
```
* 2.3.6 Update repository, working on dynamic Nuget hosting and deployment.
* 2.3.5 Streamline scene UI
* 2.3.1 Fix Breaking Changes
* 2.3.0 Thunderstore Release
```