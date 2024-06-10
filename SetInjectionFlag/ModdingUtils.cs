using BepInEx;
using BepInEx.Logging;
using PluginUtilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

namespace ModdingTales
{
    public static class ModdingUtils
    {
        private static readonly HashSet<(BaseUnityPlugin, string)> ParentPlugins = new HashSet<(BaseUnityPlugin, string)>();
        private static readonly ManualLogSource Logger = SetInjectionFlag.PluginLogger;

        /// <summary>
        /// Get a TextMeshProUGUI by name
        /// God I hate how inefficient this is but fortunately it's called rarely
        /// </summary>
        private static TextMeshProUGUI GetUITextByName(string name)
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

        /// <inheritdoc cref="AddPluginToMenuList(BaseUnityPlugin, string)"/>
        public static void AddPluginToMenuList(BaseUnityPlugin parentPlugin)
        {
            AddPluginToMenuList(parentPlugin, string.Empty);
        }

        /// <summary>
        /// Registers Plugin to be displayed in the Mod List
        /// </summary>        
        public static void AddPluginToMenuList(BaseUnityPlugin parentPlugin, string author)
        {
            ParentPlugins.Add((parentPlugin, author));
        }

        [Obsolete("See AddPluginToMenuList")]
        /// <inheritdoc cref="AddPluginToMenuList(BaseUnityPlugin, string)"/>
        public static void Initialize(BaseUnityPlugin parentPlugin, ManualLogSource logger, string author, bool startSocket = false)
        {
            AddPluginToMenuList(parentPlugin, author);
        }

        [Obsolete("See AddPluginToMenuList")]
        /// <inheritdoc cref="AddPluginToMenuList(BaseUnityPlugin, string)"/>
        public static void Initialize(BaseUnityPlugin parentPlugin, ManualLogSource logger, bool startSocket = false)
        {
            AddPluginToMenuList(parentPlugin, "");
        }

        internal static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Logger.LogDebug("Loading Scene: " + scene.name);

            TextMeshProUGUI modListText = GetUITextByName("Panel_BetaWarning");
            if (!modListText) return;

            Logger.LogDebug("Found TextMeshProUGUI in Scene: " + scene.name);

            if (modListText.text.EndsWith("</size>"))
            {
                modListText.text += "\n\nMods Currently Installed:\n";
            }

            foreach ((BaseUnityPlugin, string) parentPlugin in ParentPlugins)
            {
                try
                {
                    BepInPlugin bepInPlugin = (BepInPlugin)Attribute.GetCustomAttribute(parentPlugin.Item1.GetType(), typeof(BepInPlugin));

                    modListText.text += string.IsNullOrWhiteSpace(parentPlugin.Item2) ?
                        $"\n{bepInPlugin.Name} - {bepInPlugin.Version}" :
                        $"\n{parentPlugin.Item2} {bepInPlugin.Name} - {bepInPlugin.Version}";

                    Logger.LogDebug("Added Mod to List: " + bepInPlugin.Name);
                }
                catch (Exception e)
                {
                    Logger.LogError("Error adding mod to list: " + e.Message);
                }
            }
        }
    }
}
