using BepInEx;
using BepInEx.Logging;
using JetBrains.Annotations;
using ModdingTales;
using UnityEngine.SceneManagement;

namespace PluginUtilities
{
    [BepInPlugin(Guid, Name, Version)]
    public class SetInjectionFlag : DependencyUnityPlugin
    {
        public const string Guid = "org.generic.plugins.setinjectionflag";
        public const string Name = "Set Injection Flag Plugin";
        public const string Version = "0.0.0.0";
        internal static ManualLogSource PluginLogger;

        internal static Bounce.Localization.UiText modListText;
        internal static string originalText;

        [UsedImplicitly]
        protected override void OnAwake()
        {
            // Set App state to let BR know it's a modded instance
            Logger.LogDebug("Awake Triggered");
            PluginLogger = Logger;
            AppStateManager.UsingCodeInjection = true;
            Logger.LogInfo("Loaded, You're now good to start modding!");

            // Update UI in main menu
            ModdingUtils.AddPluginToMenuList(this);
            SceneManager.sceneLoaded += ModdingUtils.OnSceneLoaded;
            SceneManager.sceneUnloaded += ModdingUtils.OnSceneUnloaded;
        }

        /// <summary>
        /// Even if you disable the plugin, we want to keep the flag set
        /// </summary>
        [UsedImplicitly]
        protected override void OnDestroyed()
        {
            if (modListText != null)
            {
                modListText.text = originalText;
            }
        }
    }
}
