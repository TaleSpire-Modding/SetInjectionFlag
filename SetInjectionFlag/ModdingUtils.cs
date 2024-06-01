using BepInEx;
using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace ModdingTales
{
    public static class ModdingUtils
    {
        public enum LogLevel
        {
            Inherited,
            None,
            Low,
            Medium,
            High,
            All,
        }

        public static ConfigEntry<LogLevel> LogLevelConfig { get; set; }

        private static readonly Dictionary<(BaseUnityPlugin, string), ManualLogSource> ParentPlugins = new Dictionary<(BaseUnityPlugin, string), ManualLogSource>();
        
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

        public static void Initialize(BaseUnityPlugin parentPlugin, ManualLogSource logger, string author, bool startSocket = false)
        {
            AppStateManager.UsingCodeInjection = true;
            ParentPlugins.Add((parentPlugin,author), logger);
            logger.LogInfo("Inside initialize");
        }

        public static void Initialize(BaseUnityPlugin parentPlugin, ManualLogSource logger, bool startSocket=false)
        {
            AppStateManager.UsingCodeInjection = true;
            ParentPlugins.Add((parentPlugin,""),logger);
            logger.LogInfo("Inside initialize");
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Loading Scene: " + scene.name);

            foreach (var parentPlugin in ParentPlugins)
            {
                try
                {
                    var modListText = GetUITextByName("Panel_BetaWarning");
                    if (!modListText) continue;
                    var bepInPlugin =
                        (BepInPlugin)Attribute.GetCustomAttribute(parentPlugin.Key.Item1.GetType(),
                            typeof(BepInPlugin));
                    if (modListText.text.EndsWith("</size>"))
                    {
                        modListText.text += "\n\nMods Currently Installed:\n";
                    }

                    modListText.text += string.IsNullOrWhiteSpace(parentPlugin.Key.Item2) ? 
                        $"\n{bepInPlugin.Name} - {bepInPlugin.Version}" : 
                        $"\n{parentPlugin.Key.Item2} {bepInPlugin.Name} - {bepInPlugin.Version}";
                }
                catch (Exception ex)
                {
                    parentPlugin.Value.LogFatal(ex);
                }
            }
        }
    }
}
