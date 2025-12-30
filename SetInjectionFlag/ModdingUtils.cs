using BepInEx;
using BepInEx.Logging;
using PluginUtilities;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace ModdingTales
{
    public static class ModdingUtils
    {
        private static readonly HashSet<BaseUnityPlugin> ParentPlugins = new HashSet<BaseUnityPlugin>();
        private static readonly ManualLogSource Logger = SetInjectionFlag.PluginLogger;

        /// <summary>
        /// Get a TextMeshProUGUI by name
        /// God I hate how inefficient this is but fortunately it's called rarely
        /// </summary>
        private static Bounce.Localization.UiText GetUITextByName(string name)
        {
            Bounce.Localization.UiText[] texts = UnityEngine.Object.FindObjectsOfType<Bounce.Localization.UiText>();
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i].name == name)
                {
                    return texts[i];
                }
            }
            return null;
        }

        [Obsolete("This automatically gets called for plugins that inherit DependencyUnityPlugin on Awake")]
        public static void AddPluginToMenuList(DependencyUnityPlugin parentPlugin)
        {
            AddPluginToMenuList(parentPlugin, string.Empty);
        }

        [Obsolete("This automatically gets called for plugins that inherit DependencyUnityPlugin on Awake")]
        public static void AddPluginToMenuList(DependencyUnityPlugin parentPlugin, string author)
        {
            if (ParentPlugins.Contains(parentPlugin))
                return;

            ParentPlugins.Add(parentPlugin);
            RefreshUIList();

            parentPlugin.Destroyed += () =>
            {
                RemovePluginFromMenuList(parentPlugin);
            };
        }

        [Obsolete("This automatically gets called for plugins that inherit DependencyUnityPlugin on Awake")]
        public static void AddPluginToMenuList(BaseUnityPlugin parentPlugin)
        {
            ParentPlugins.Add(parentPlugin);
            RefreshUIList();
        }

        [Obsolete("This automatically gets called for plugins that inherit DependencyUnityPlugin on Awake")]
        public static void AddPluginToMenuList(BaseUnityPlugin parentPlugin, string author)
        {
            AddPluginToMenuList(parentPlugin);
        }

        [Obsolete("This automatically gets called for plugins that inherit DependencyUnityPlugin on Destroy")]
        public static void RemovePluginFromMenuList(BaseUnityPlugin parentPlugin)
        {
            ParentPlugins.Remove(parentPlugin);
            RefreshUIList();
        }

        [Obsolete("This automatically gets called for plugins that inherit DependencyUnityPlugin on Destroy")]
        public static void RemovePluginFromMenuList(BaseUnityPlugin parentPlugin, string author)
        {
            RemovePluginFromMenuList(parentPlugin);
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

        internal static void OnSceneUnloaded(Scene scene)
        {
            if (scene == null || scene.name != "Login")
                return;
            SetInjectionFlag.modListText = null;
            SetInjectionFlag.originalText = null;
        }

        internal static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene == null || scene.name != "Login")
                return;

            Bounce.Localization.UiText modListText = GetUITextByName("Copyright Text");
            SetInjectionFlag.modListText = modListText; 
            if (!modListText) return;
            Logger.LogDebug("Found TextMeshProUGUI in Scene: " + scene.name);

            SetInjectionFlag.originalText = modListText.text;

            RefreshUIList();
        }

        /// <summary>
        /// Updates the UI list of mods if present
        /// </summary>
        internal static void RefreshUIList()
        {
            if (SetInjectionFlag.modListText == null) 
                return;

            SetInjectionFlag.modListText.text = SetInjectionFlag.originalText;

            // legacy check to avoid duplicating the header
            if (SetInjectionFlag.modListText.text.EndsWith("</size>"))
            {
                SetInjectionFlag.modListText.text += "\n\nInstalled Mods:";
            }

            foreach (BaseUnityPlugin parentPlugin in ParentPlugins)
            {
                try
                {
                    BepInPlugin bepInPlugin = (BepInPlugin)Attribute.GetCustomAttribute(parentPlugin.GetType(), typeof(BepInPlugin));

                    // for now we are ignoring the author name passed in
                    SetInjectionFlag.modListText.text += $"\n<indent=5%>{bepInPlugin.Name} - {bepInPlugin.Version}</indent>";
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
