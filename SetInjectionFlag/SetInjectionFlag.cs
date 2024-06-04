using BepInEx;
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
        public const string Version = "0.0.0.0";

        [UsedImplicitly]
        private void Awake()
        {
            Logger.LogDebug("Awake Triggered");
            Logger.LogInfo("Loaded, You're now good to start modding!");
            ModdingUtils.Initialize(this);
            SceneManager.sceneLoaded += ModdingUtils.OnSceneLoaded;
        }
    }
}
