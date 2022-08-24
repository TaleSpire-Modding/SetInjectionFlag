using BepInEx;
using BepInEx.Configuration;
using JetBrains.Annotations;
using ModdingTales;
using UnityEngine.SceneManagement;

namespace PluginUtilities
{
    [BepInPlugin(Guid, Name, Version)]
    public class SetInjectionFlag : BaseUnityPlugin
    {
        public const string Guid = "org.generic.plugins.setinjectionflag";
        public const string Name = "Set Injection Flag Plugin";
        public const string Version = "2.3.4.0";

        
        public static void DoConfig(ConfigFile config)
        {
            ModdingUtils.LogLevelConfig = config.Bind("Logging", "Level", ModdingUtils.LogLevel.Low);
            if (ModdingUtils.LogLevelConfig.Value != ModdingUtils.LogLevel.Inherited) return;
            ModdingUtils.LogLevelConfig.Value = ModdingUtils.LogLevel.None;
            UnityEngine.Debug.Log("Logging level set to None, Inherited is for child plugins");
        }

        [UsedImplicitly]
        private void Awake()
        {
            Logger.LogInfo("In Awake for SetInjectionFlag Plug-in");
            UnityEngine.Debug.Log("SetInjectionFlag Plug-in loaded");
            ModdingUtils.Initialize(this, Logger);
            SceneManager.sceneLoaded += ModdingUtils.OnSceneLoaded;
            DoConfig(Config);
        }
    }
}