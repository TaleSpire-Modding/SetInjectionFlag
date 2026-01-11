using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using JetBrains.Annotations;
using ModdingTales;
using UnityEngine.SceneManagement;

namespace PluginUtilities
{
    [BepInPlugin(Guid, Name, Version)]
    public class SetInjectionFlag : DependencyUnityPlugin<SetInjectionFlag>
    {
        public const string Guid = "org.generic.plugins.setinjectionflag";
        public const string Name = "Set Injection Flag Plugin";
        public const string Version = "0.0.0.0";
        internal static ManualLogSource PluginLogger;

        internal static Bounce.Localization.UiText modListText;
        internal static string originalText;

        /// <summary>
        /// setups config even if not enabled
        /// </summary>
        protected override void OnSetupConfig(ConfigFile config)
        {
            // Doing an early override to garauntee this is enabled.
            // DO NOT DO THIS WITH OTHER PLUGINS
            if (!EnabledConfig())
            {
                Logger.LogWarning("Overriding disabled state to enabled for modding purposes");
                PluginEnabled.Value = true;
                config.Save();
            }
        }

        [UsedImplicitly]
        protected override void OnAwake()
        {
            // Set App state to let BR know it's a modded instance
            Logger.LogDebug("Awake Triggered");
            PluginLogger = Logger;
            Logger.LogInfo("Loaded, You're now good to start modding!");

            AppStateManager.UsingCodeInjection = true;

            // Update UI in main menu
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

            SceneManager.sceneLoaded -= ModdingUtils.OnSceneLoaded;
            SceneManager.sceneUnloaded -= ModdingUtils.OnSceneUnloaded;

            // re-enable the plugin if was disabled
            if (!Enabled)
            {
                Logger.LogError("You Disabled this Plugin when you shouldn't have");
                PluginEnabled.Value = true;
            }
        }
    }
}
