using UnityEngine;
using BepInEx;
using UnityEngine.SceneManagement;
using System;
using BepInEx.Logging;
using TMPro;

namespace ModdingTales
{
    public static class ModdingUtils
    {
        private static BaseUnityPlugin parentPlugin;
        private static ManualLogSource parentLogger;
        
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

        public static void Initialize(BaseUnityPlugin parentPlugin, ManualLogSource logger, bool startSocket=false)
        {
            AppStateManager.UsingCodeInjection = true;
            ModdingUtils.parentPlugin = parentPlugin;
            parentLogger = logger;
            parentLogger.LogInfo("Inside initialize");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            try
            {
                parentLogger.LogInfo("On Scene Loaded" + scene.name);
                Debug.Log("Loading Scene: " + scene.name);
                if (scene.name == "UI") {
                    TextMeshProUGUI betaText = GetUITextByName("BETA");
                    if (betaText)
                    {
                        betaText.text = "INJECTED BUILD - unstable mods";
                    }
                } else 
                {
                    TextMeshProUGUI modListText = GetUITextByName("TextMeshPro Text");
                    if (modListText)
                    {
                        BepInPlugin bepInPlugin = (BepInPlugin)Attribute.GetCustomAttribute(ModdingUtils.parentPlugin.GetType(), typeof(BepInPlugin));
                        if (modListText.text.EndsWith("</size>"))
                        {
                            modListText.text += "\n\nMods Currently Installed:\n";
                        }
                        modListText.text += "\n" + bepInPlugin.Name + " - " + bepInPlugin.Version;
                    }
                }
            }
            catch (Exception ex)
            {
                parentLogger.LogFatal(ex);
            }
        }
    }
}
