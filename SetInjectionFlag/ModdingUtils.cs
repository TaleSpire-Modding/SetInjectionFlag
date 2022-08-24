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

        private static readonly Dictionary<BaseUnityPlugin, ManualLogSource> ParentPlugins = new Dictionary<BaseUnityPlugin, ManualLogSource>();

        public static TextMeshProUGUI GetUITextByName(string name)
        {
            TextMeshProUGUI[] texts = UnityEngine.Object.FindObjectsOfType<TextMeshProUGUI>();
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i].name == name)
                {
                    return texts[i];
                }
            }
            return null;
        }

        public static void Initialize(BaseUnityPlugin parentPlugin, ManualLogSource logger, bool startSocket=false)
        {
            AppStateManager.UsingCodeInjection = true;
            ParentPlugins.Add(parentPlugin,logger);
            logger.LogInfo("Inside initialize");
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (var parentPlugin in ParentPlugins)
            {
                try
                {
                    parentPlugin.Value.LogInfo("On Scene Loaded " + scene.name);
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
                            (BepInPlugin)Attribute.GetCustomAttribute(parentPlugin.Key.GetType(),
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
                    parentPlugin.Value.LogFatal(ex);
                }
            }
        }
    }
}
