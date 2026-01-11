# Set Injection Flag Plugin 
[![Push to nuget feed on release](https://github.com/TaleSpire-Modding/SetInjectionFlag/actions/workflows/release.yml/badge.svg)](https://github.com/TaleSpire-Modding/SetInjectionFlag/actions/workflows/release.yml)

This is a plugin for TaleSpire using BepInEx. This plugin lets BouncyRock developers know that logs being generated are from vanilla or modded instances.
There is additional functionality for developers to enable dynamic auto-binding of plugin dependencies via Bepinex Chainloader.

## Install

Easiest way to install this is via R2Modman

## Usage

Just install, this will automatically update TaleSpire so the devs know you are doing modding work.

## How to Compile / Modify

Open ```SetInjectionFlagPlugin.sln``` in Visual Studio.

Build the project (Now using Nuget for package dependency).

Browse to the newly created ```bin/Debug``` or ```bin/Release``` folders and copy the ```SetInjectionFlagPlugin.dll``` to ```Steam\steamapps\common\TaleSpire\BepInEx\plugins```

## Developer usage of DependencyUnityPlugin<T>
There is a new generic abstract class `DependencyUnityPlugin<T>` that can be used instead of `BaseUnityPlugin` to help manage dependencies and plugin lifecycle better.
This class implements some new virtual methods that can be overridden to handle configuration setup, Awake and Destroy events that respects the bepinex chainloader.
By using the generic abstract class, Bepinex ChainLoader is respected whenever plugins are dynamically added/removed from the mod list, and the plugin can be enabled/disabled via config.
This is designed so the developer does not need to implement checks for whether the plugin is enabled/disabled or not, and can focus on the actual plugin logic.

```CSharp
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ModdingTales;
using PluginUtilities;

	// Before/Basic Implementation without Chainload binding or enabling/disabling
	[BepInPlugin(Guid, Name, Version)]
	[BepInDependency(SetInjectionFlag.Guid)]
	MyPluginA: BaseUnityPlugin
	{
		// Consts to describe the plugin
		public const string Guid = "Your.PluginA.Guid.Here";
        public const string Name = "Your PluginA Name";
        public const string Version = "0.0.0.0";

		// Config Entries
		internal static ConfigEntry<int> Value;

	    void Awake()
	    {
			Value = Config.Bind("General", nameof(Value), 100);

	        Logger.LogInfo($"In Awake for {Name}");
            harmony = new Harmony(Guid);
            harmony.PatchAll();

			// Registers Plugin in Mod List UI (Automatically added/removed if use DependencyUnityPlugin or DependencyUnityPlugin<T>)
			ModdingUtils.AddPluginToMenuList(this);

			// Do Some Other Work Here
	    }

	}

	// After/Proper Implementation with Chainload binding and enabling/disabling plus config setup and lifecycle management
	[BepInPlugin(Guid, Name, Version)]
	[BepInDependency(SetInjectionFlag.Guid)]
	MyPluginB: DependencyUnityPlugin<MyPlugin>B
	{   
		// Consts to describe the plugin
		public const string Guid = "Your.PluginB.Guid.Here";
        public const string Name = "Your PluginB Name";
        public const string Version = "0.0.0.0";

		// tracking Harmony here for unpatching on destroy
		internal static Harmony harmony;

		// Config Entries
		internal static ConfigEntry<int> Value;


		// Config Entry Callbacks
		private static void ConfigEntryUpdateCallback(object _)
		{
			
			// Something to do when config entry is updated (not to be confused with enabled which is from UnityEngine.Behaviour)
			if (Enabled) {
				// Plugin is enabled so we should do something
			} else {
				// Plugin is disabled so we can ignore the callback
			}
		}


		// Implement your config binding here
		protected override void OnSetupConfig(ConfigFile config)
        {
            // Create attribute for config entry
			ConfigurationAttributes configurationAttributes = new ConfigurationAttributes
            {
                CallbackAction = ConfigEntryUpdateCallback
            };

			// Create description for config entry
			ConfigDescription myConfigDescription = new ConfigDescription("", null, configurationAttributes);

			// Bind with description and attributes
			Value = Config.Bind("General", nameof(Value), 100, myConfigDescription);
        }


		// Implement code to run on Awake here (if the plugin is enabled)
		protected override void OnAwake()
        {
            Logger.LogInfo($"In Awake for {Name}");
            harmony = new Harmony(Guid);
            harmony.PatchAll();

			// Do Some Other Work Here
        }

		// Implement code to run on Destroy here (if the plugin is disabled/removed)
		protected override void OnDestroyed()
        {
            harmony.UnpatchSelf();

			// Cleanup resources here
        }
	}
```

## Changelog
```
* 3.4.1 Fix System reflection issue with DependencyUnityPlugin generic type constraint.
* 3.4.0 Add configuration option to enable/disable the plugin without uninstalling it via DependencyUnityPlugin<T>.
* 3.3.4 Optimization for scene load/unload. DependencyUnityPlugin now auto-subscribes to SIF.
* 3.3.3 Fixed an issue where a DependencyUnityPlugin could be called to destroy multiple times.
* 3.3.2 DependencyUnityPlugins are now used to subscribe when those are destroyed too if/when appended to the mod list.
* 3.3.1 Changed Destroyed Action to public for compatibility with other/future mods.
* 3.3.0 Implement DependencyUnityPlugin to manage dependencies better. Fixed some minor issues with the mod list UI.
* 3.2.0 Re-implemented the UI Mod List. This is now dynamically updated when mods are added/removed from the list.
* 3.1.0 Added ConfigurationAttributes to allow extra information to be appended to configs and usable for ConfigManager in the future
* 3.0.0 Recompile to keep up to date, no feature changes.
* 2.6.0 Logging upgrade, Migrate to use BepInEx logging framework instead
* 2.5.2 Seats Fix
* 2.5.1 TaleWeaver Lite Fix
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
