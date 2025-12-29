using BepInEx;
using BepInEx.Bootstrap;
using JetBrains.Annotations;
using System;
using System.Linq;

namespace PluginUtilities
{
    public abstract class DependencyUnityPlugin : BaseUnityPlugin
    {
        // Action to subscribe to for when this plugin is destroyed
        private Action Destroyed;

        public static DependencyUnityPlugin[] GetPluginsForDependencies(Type type)
        {
            // Get all dependency attributes on the class
            var deps = type
                .GetCustomAttributes(typeof(BepInDependency), inherit: true)
                .Cast<BepInDependency>();

            // Match to loaded plugins
            return deps
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
                dep.Destroyed += () => {
                    Destroy(this);
                };
            }

            OnAwake();
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
