using BepInEx;
using BepInEx.Bootstrap;
using JetBrains.Annotations;
using ModdingTales;
using System;
using System.Linq;

namespace PluginUtilities
{
    public abstract class DependencyUnityPlugin : BaseUnityPlugin
    {
        // Action to subscribe to for when this plugin is destroyed
        public Action Destroyed;

        public static DependencyUnityPlugin[] GetPluginsForDependencies(Type type)
        {
            // Match to loaded plugins
            return type
                .GetCustomAttributes(typeof(BepInDependency), inherit: true)
                .Cast<BepInDependency>()
                .Where(d => Chainloader.PluginInfos.ContainsKey(d.DependencyGUID))
                .Select(d => Chainloader.PluginInfos[d.DependencyGUID].Instance)
                .Where(d => d is DependencyUnityPlugin)
                .Select(d => d as DependencyUnityPlugin)
                .ToArray();
        }

        [UsedImplicitly]
        protected void Awake()
        {
            var deps = GetPluginsForDependencies(GetType());
            foreach (var dep in deps)
            {
                // When a dependency is destroyed, destroy this plugin as well
                dep.Destroyed += DestroyAndUnbind;
            }

            OnAwake();
            ModdingUtils.AddPluginToMenuList(this);
        }

        // Called when a dependency is destroyed to clean up this plugin
        private void DestroyAndUnbind()
        {
            var deps = GetPluginsForDependencies(GetType());
            foreach (var dep in deps)
            {
                // When a dependency is destroyed, unbind itself to prevent recall
                dep.Destroyed -= DestroyAndUnbind;
            }

            Destroy(this);
        }

        /// <summary>
        /// base cleanup
        /// </summary>
        protected virtual void OnAwake()
        {
            // base cleanup
        }

        /// <summary>
        /// Even if you disable the plugin, we want to keep the flag set
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
