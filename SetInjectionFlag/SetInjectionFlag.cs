using BepInEx;
using ModdingTales;

namespace PluginUtilities
{
    [BepInPlugin(Guid, Name, Version)]
    class SetInjectionFlag : BaseUnityPlugin
    {
        public const string Guid = "org.generic.plugins.setinjectionflag";
        public const string Name = "Set Injection Flag Plugin";
        public const string Version = "1.3.0.0";


        void Awake()
        {
            Logger.LogInfo("In Awake for SetInjectionFlag Plug-in");

            UnityEngine.Debug.Log("SetInjectionFlag Plug-in loaded");
            ModdingUtils.Initialize(this, this.Logger);
        }
    }
}