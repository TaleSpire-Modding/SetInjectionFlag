using UnityEngine;
using BepInEx;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using BepInEx.Logging;
using JetBrains.Annotations;
using TMPro;

namespace ModdingTales
{
    public static class ModdingUtils
    {
        public enum LogLevel
        {
            [UsedImplicitly] Inherited,
            [UsedImplicitly] None,
            [UsedImplicitly] Low,
            [UsedImplicitly] Medium,
            [UsedImplicitly] High,
            [UsedImplicitly] All,
        }

        public static ConfigEntry<LogLevel> LogLevelConfig { get; set; }

        private static readonly List<BaseUnityPlugin> ParentPlugins = new List<BaseUnityPlugin>();

        private static ManualLogSource _parentLogger;
        
        public static TextMeshProUGUI GetUITextByName(string name)
         => UnityEngine.Object.FindObjectsOfType<TextMeshProUGUI>().SingleOrDefault(t => t.name == name);

        public static void Initialize(BaseUnityPlugin parentPlugin, ManualLogSource logger, bool startSocket=false)
        {
            AppStateManager.UsingCodeInjection = true;
            ParentPlugins.Add(parentPlugin);
            _parentLogger = logger;
            _parentLogger.LogInfo("Inside initialize");
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (var parentPlugin in ParentPlugins)
            {
                try
                {
                    _parentLogger.LogInfo("On Scene Loaded" + scene.name);
                    Debug.Log("Loading Scene: " + scene.name);
                    if (scene.name == "UI")
                    {
                        var betaText = GetUITextByName("BETA");
                        if (betaText)
                        {
                            betaText.text = "INJECTED BUILD - unstable mods";
                        }
                    }
                    else
                    {
                        var modListText = GetUITextByName("TextMeshPro Text");
                        if (!modListText) continue;
                        var bepInPlugin =
                            (BepInPlugin)Attribute.GetCustomAttribute(parentPlugin.GetType(),
                                typeof(BepInPlugin));
                        if (modListText.text.EndsWith("</size>"))
                        {
                            modListText.text += "\n\nMods Currently Installed:\n";
                        }
                        modListText.text += "\n" + bepInPlugin.Name + " - " + bepInPlugin.Version;
                    }
                }
                catch (Exception ex)
                {
                    _parentLogger.LogFatal(ex);
                }
            }
        }
    }
}
