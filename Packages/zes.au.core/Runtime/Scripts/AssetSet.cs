﻿using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Diagnostics;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

namespace Au
{
    /// <summary>
    /// Asset set
    /// </summary>
    public class AssetSet
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="basePath">assets baseurl, relative to persistent data path</param>
        public AssetSet(string basePath)
        {
            this.basePath = Path.Combine(Application.persistentDataPath, basePath);
        }

        private readonly string basePath;
        private readonly Dictionary<string, Pending> pendings = new Dictionary<string, Pending>();
        private readonly Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
        private readonly Dictionary<string, Object> assets = new Dictionary<string, Object>();
        private Dictionary<string, string> assets2bundle = new Dictionary<string, string>();
        private readonly Log log = Log.GetLogger<AssetSet>();

        /// <summary>
        /// Load a bundle 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public async Task<bool> LoadBundle(string name, System.Action<float> progress)
        {
            if (bundles.TryGetValue(name, out var bundle))
            {
                progress?.Invoke(1);
                return bundle;
            }

#if UNITY_EDITOR && !USING_BUNDLE
            return await LoadBundleEditor(name, progress);
#else
            return await LoadBundleRuntime(name, progress);
#endif
        }

        /// <summary>
        /// Unload bundle
        /// </summary>
        /// <param name="name"></param>
        public void UnloadBundle(string name)
        {
            if (bundles.TryGetValue(name, out var bundle))
            {
                bundles.Remove(name);
                assets2bundle = assets2bundle.Where(i => i.Value != name).ToDictionary(i => i.Key, i => i.Value);
                bundle?.Unload(true);
            }
        }

        /// <summary>
        /// Load an object from loaded bundle
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<Object> LoadObject(string path, System.Type type)
        {
            if (assets.TryGetValue(path, out var obj))
            {
                return obj;
            }
#if UNITY_EDITOR && !USING_BUNDLE
            return await LoadObjectEditor(path, type);
#else
            return await LoadObjectRuntime(path, type);
#endif

        }

#if UNITY_EDITOR
        private async Task<bool> LoadBundleEditor(string name, System.Action<float> progress)
        {
            await Task.Yield();
            bundles.Add(name, null);
            progress?.Invoke(1);
            return true;
        }

        private async Task<Object> LoadObjectEditor(string path, System.Type type)
        {
            await Task.Yield();
            var obj = AssetDatabase.LoadAssetAtPath(path, type);
            if (obj == null)
            {
                log.Error($"load {path} failed !");
                return null;
            }
            assets.Add(path, obj);
            return obj;
        }
#endif

        private async Task<bool> LoadBundleRuntime(string name, System.Action<float> progress)
        {
            var data = await PendingLock("bundle", name, async () =>
            {
                string fullpath = Path.Combine(basePath, name);
                if (!Files.Exists(fullpath))
                {
                    log.Error($"{fullpath} not exists.");
                    return null;
                }
                var req = AssetBundle.LoadFromFileAsync(fullpath);
                await Async.WaitAsyncOperation(req, progress);
                var bundle = req.assetBundle;
                bundles.Add(name, req.assetBundle);
                if (!bundle.isStreamedSceneAssetBundle)
                {
                    bundle.GetAllAssetNames()
                        .ToList()
                        .ForEach(i => assets2bundle.Add(i.ToLower(), name));
                }
                progress?.Invoke(1);
                return bundle;
            });
            return data != null;
        }

        private async Task<Object> LoadObjectRuntime(string path, System.Type type)
        {
            var obj = await PendingLock("asset", path, async () =>
            {
                path = path.ToLower();
                if (!assets2bundle.TryGetValue(path, out var item))
                {
                    log.Error($"Cannot find bundle for {path}");
                    return null;
                }

                if (!bundles.TryGetValue(item, out var bundle))
                {
                    log.Error($"Bundle {item} not loaded");
                    return null;
                }

                Debug.Assert(bundle != null, $"bundle ({item}) cannot be null");
                var op = bundle.LoadAssetAsync(path, type);
                await Async.WaitAsyncOperation(op);
                assets.Add(path, op.asset);
                return op.asset;
            });

            return obj;
        }

        private async Task<T> PendingLock<T>(string type, string id, System.Func<Task<T>> func)
        {
            string key = $"{type}_{id}";
            if (pendings.TryGetValue(key, out var item))
            {
                await Async.WaitUntil(() => item.finished);
                return (T)item.data;
            }

            item = new Pending();
            pendings.Add(key, item);
            var data = await func();
            item.data = data;
            pendings.Remove(key);
            return data;
        }

        class Pending
        {
            public string key;
            public bool finished = false;
            public bool succ;
            public object data;
        }
    }
}