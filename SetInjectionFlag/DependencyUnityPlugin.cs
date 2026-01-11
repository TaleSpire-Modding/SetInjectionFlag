using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using JetBrains.Annotations;
using ModdingTales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PluginUtilities
{
    public abstract class DependencyUnityPlugin<T> : DependencyUnityPlugin, IDependencyUnityPlugin
        where T : DependencyUnityPlugin<T>
    {
        internal static ConfigEntry<bool> PluginEnabled;

        public static bool Enabled { get => PluginEnabled.Value; }

        private static GameObject PluginGameObject { get; set; }

        private static Type[] RequiredPlugins;
        private static readonly HashSet<Type> DependantPlugins = new HashSet<Type>();

        private static void EnabledChanged(object _)
        {
            if (Enabled)
                Enable();
            else
                Disable();
        }

        protected static void Enable() 
        {
            if (PluginGameObject.GetComponent<T>() == null)
            {
                PluginEnabled.Value = true;

                foreach (Type depType in RequiredPlugins)
                {
                    MethodInfo enableMethod = depType.GetMethod(nameof(Enable), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    enableMethod?.Invoke(null, null);
                }

                PluginGameObject.AddComponent<T>();
            }
        }

        protected static void Disable() 
        {
            if (PluginGameObject.GetComponent<T>() != null)
            {
                PluginEnabled.Value = false;

                foreach (Type depType in DependantPlugins)
                {
                    MethodInfo disableMethod = depType.GetMethod(nameof(Disable), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    disableMethod?.Invoke(null, null);
                }

                PluginGameObject.GetComponent<T>()?.DestroyAndUnbind();
            }
        }

        internal override bool EnabledConfig()
        {
            PluginGameObject = gameObject;

            ConfigurationAttributes configurationAttributes = new ConfigurationAttributes
            {
                CallbackAction = EnabledChanged
            };

            ConfigDescription enabledPluginDescription = new ConfigDescription("", null, configurationAttributes);

            PluginEnabled = Config.Bind("General", nameof(Enabled), true, enabledPluginDescription);

            IEnumerable<DependencyUnityPlugin> dep = GetPluginsForDependencies(GetType()).Where(d => d is IDependencyUnityPlugin);
            RequiredPlugins = dep.Select(s => s.GetType()).ToArray();

            foreach (Type depType in RequiredPlugins)
            {
                Logger.LogDebug($"Adding required plugin {depType.FullName} to {typeof(T).FullName}");
            }

            foreach (IDependencyUnityPlugin d in dep.Cast<IDependencyUnityPlugin>()) {
                d.AddDep(this);
            }

            return PluginEnabled.Value;
        }

        void IDependencyUnityPlugin.AddDep(IDependencyUnityPlugin dep)
        {
            Type depType = dep.GetType();
            if (!DependantPlugins.Contains(depType))
            {
                Logger.LogDebug($"Adding dependant plugin {depType.FullName} to {typeof(T).FullName}");
                DependantPlugins.Add(depType);
            }
        }

        public void OnValueChanged(object _)
        {
            throw new NotImplementedException();
        }
    }

    public interface IDependencyUnityPlugin
    {
        void AddDep(IDependencyUnityPlugin dep);
    }

    public abstract class DependencyUnityPlugin : BaseUnityPlugin
    {
        // Action to subscribe to for when this plugin is destroyed
        public Action Destroyed;

        protected DependencyUnityPlugin() 
        {

        }

        internal virtual bool EnabledConfig()
        {
            return true;
        }

        public static DependencyUnityPlugin[] GetPluginsForDependencies(Type type)
        {
            // Match to loaded plugins
            return type
                .GetCustomAttributes(typeof(BepInDependency), inherit: true)
                .Cast<BepInDependency>()
                .Where(d => Chainloader.PluginInfos.ContainsKey(d.DependencyGUID))
                .Select(d => Chainloader.PluginInfos[d.DependencyGUID].Instance)
                .OfType<DependencyUnityPlugin>()
                .ToArray();
        }
        
        [UsedImplicitly]
        protected void Awake()
        {
            // setup config
            OnSetupConfig(Config);

            // check if enabled otherwise destroy
            if (EnabledConfig())
            {
                DependencyUnityPlugin[] deps = GetPluginsForDependencies(GetType());
                foreach (DependencyUnityPlugin dep in deps)
                {
                    // When a dependency is destroyed, destroy this plugin as well
                    dep.Destroyed += DestroyAndUnbind;
                }

                OnAwake();
#pragma warning disable CS0618 // Type or member is obsolete
                ModdingUtils.AddPluginToMenuList(this);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            else
            {
                DestroyAndUnbind();
            }
        }

        
        // Called when a dependency is destroyed to clean up this plugin
        internal void DestroyAndUnbind()
        {
            DependencyUnityPlugin[] deps = GetPluginsForDependencies(GetType());
            foreach (DependencyUnityPlugin dep in deps)
            {
                // When a dependency is destroyed, unbind itself to prevent recall
                dep.Destroyed -= DestroyAndUnbind;
            }

            Destroy(this);
        }

        /// <summary>
        /// setups config even if not enabled
        /// </summary>
        protected virtual void OnSetupConfig(ConfigFile config)
        {
            // base setup
        }

        /// <summary>
        /// base awake
        /// </summary>
        protected virtual void OnAwake()
        {
            // base awake
        }

        /// <summary>
        /// Signals to dependent plugins that this plugin is being destroyed then cleans up
        /// </summary>
        [UsedImplicitly]
        protected void OnDestroy()
        {
            // signify dependencies to destroy themselves first and cleanup before us
            Destroyed?.Invoke();
            OnDestroyed();
        }

        /// <summary>
        /// base cleanup
        /// </summary>
        protected virtual void OnDestroyed()
        {
            // base cleanup
        }
    }
}
