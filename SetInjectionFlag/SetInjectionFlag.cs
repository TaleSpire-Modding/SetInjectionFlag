using BepInEx;
using ModdingTales;
using UnityEngine.SceneManagement;

namespace PluginUtilities
{
    [BepInPlugin(Guid, Name, Version)]
    public class SetInjectionFlag : BaseUnityPlugin
    {
        public const string Guid = "org.generic.plugins.setinjectionflag";
        public const string Name = "Set Injection Flag Plugin";
        public const string Version = "2.3.2.0";


        void Awake()
        {
            Logger.LogInfo("In Awake for SetInjectionFlag Plug-in");

            UnityEngine.Debug.Log("SetInjectionFlag Plug-in loaded");
            ModdingUtils.Initialize(this, this.Logger);
            SceneManager.sceneLoaded += ModdingUtils.OnSceneLoaded;
        }
    }
}