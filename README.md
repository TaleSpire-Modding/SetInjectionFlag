# Set Injection Flag Plugin 
[![Push to nuget feed on release](https://github.com/TaleSpire-Modding/SetInjectionFlag/actions/workflows/release.yml/badge.svg)](https://github.com/TaleSpire-Modding/SetInjectionFlag/actions/workflows/release.yml)

This is a plugin for TaleSpire using BepInEx. This plugin lets BouncyRock developers know that logs being generated are from vanilla or modded instances.

## Install

Easiest way to install this is via R2Modman

## Usage

Just install, this will automatically update TaleSpire so the devs know you are doing modding work.

## How to Compile / Modify

Open ```SetInjectionFlagPlugin.sln``` in Visual Studio.

Build the project (Now using Nuget for package dependency).

Browse to the newly created ```bin/Debug``` or ```bin/Release``` folders and copy the ```SetInjectionFlagPlugin.dll``` to ```Steam\steamapps\common\TaleSpire\BepInEx\plugins```

## Changelog
```
* 2.5.0 Updated build, depenencies and to use the most up to date Talespire dlls for compatibility.
* 2.4.3 bump version and fix wrapping
* 2.4.2 Test deploy using pipeline
* 2.4.1 Updating package details and dependency
* 2.4.0 Package released on Github and Nuget
* 2.4.0 Framework upgrade from 4.7.2 Tool to 4.8 SDK
* 2.3.6 Update repository, working on dynamic Nuget hosting and deployment.
* 2.3.5 Streamline scene UI
* 2.3.1 Fix Breaking Changes
* 2.3.0 Thunderstore Release
```
