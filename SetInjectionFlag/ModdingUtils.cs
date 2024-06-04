using BepInEx;
using BepInEx.Logging;
using PluginUtilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

namespace ModdingTales
{
    public abstract class BepinexLogging
    {
        protected ManualLogSource Logger { get; }

        protected BepinexLogging()
        {
            string LoggerID = GetType().AssemblyQualifiedName;
            Logger = BepInEx.Logging.Logger.CreateLogSource(LoggerID);
        }
    }

    public static class ModdingUtils
    {
        private static readonly HashSet<(BaseUnityPlugin, string)> ParentPlugins = new HashSet<(BaseUnityPlugin, string)>();
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource($"{nameof(SetInjectionFlag)}.{nameof(ModdingUtils)}");

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

        /// <inheritdoc cref="Initialize(BaseUnityPlugin, string)"/>
        public static void Initialize(BaseUnityPlugin parentPlugin)
        {
            Initialize(parentPlugin, string.Empty);
        }

        /// <summary>
        /// Registers Plugin to be displayed in the Mod List
        /// </summary>        
        public static void Initialize(BaseUnityPlugin parentPlugin, string author)
        {
            AppStateManager.UsingCodeInjection = true;
            ParentPlugins.Add((parentPlugin, author));
        }

        [Obsolete]
        /// <inheritdoc cref="Initialize(BaseUnityPlugin, string)"/>
        public static void Initialize(BaseUnityPlugin parentPlugin, ManualLogSource logger, string author, bool startSocket = false)
        {
            Initialize(parentPlugin, author);
        }

        [Obsolete]
        /// <inheritdoc cref="Initialize(BaseUnityPlugin, string)"/>
        public static void Initialize(BaseUnityPlugin parentPlugin, ManualLogSource logger, bool startSocket = false)
        {
            Initialize(parentPlugin, "");
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
                BepInPlugin bepInPlugin = (BepInPlugin) Attribute.GetCustomAttribute(parentPlugin.Item1.GetType(),typeof(BepInPlugin));

                modListText.text += string.IsNullOrWhiteSpace(parentPlugin.Item2) ? 
                    $"\n{bepInPlugin.Name} - {bepInPlugin.Version}" : 
                    $"\n{parentPlugin.Item2} {bepInPlugin.Name} - {bepInPlugin.Version}";

                Logger.LogDebug("Added Mod to List: " + bepInPlugin.Name);
            }
        }
    }
}
