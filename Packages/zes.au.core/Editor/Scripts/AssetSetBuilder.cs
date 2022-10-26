using System.IO;
using System.Linq;
using UnityEditor;

namespace Au
{
    public static class AssetSetBuilder
    {
        private static Log log = Log.GetLogger("AssetSetBuilder");

        /// <summary>
        /// Auto create bundle names in dir Assets/{path}
        /// Assume each sub-dir is a bundle
        /// </summary>
        /// <param name="path"></param>
        public static void AutoCreateBundleNames(string path)
        {
            var bundles = new DirectoryInfo(path);
            var dirs = bundles.GetDirectories();
            foreach (var dir in dirs)
            {
                var importer = AssetImporter.GetAtPath(Path.Combine(path, dir.Name));
                if (importer == null)
                {
                    log.Error($"AssetImporter is null : {Path.Combine(path, dir.Name)}");
                    continue;
                }
                importer.assetBundleName = dir.Name.ToLower();
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Clear all bundle names
        /// </summary>
        /// <param name="path"></param>
        public static void ClearBundleNames(string path = "Assets")
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                return;
            }

            dir.GetFiles()
                .Select(file => AssetImporter.GetAtPath(file.FullName))
                .Where(i => i != null)
                .Where(i => !string.IsNullOrEmpty(i.assetBundleName))
                .ToList()
                .ForEach(i => i.assetBundleName = string.Empty);


            dir.GetDirectories()
                .Select(d => AssetImporter.GetAtPath(d.FullName))
                .Where(i => i != null)
                .Where(i => !string.IsNullOrEmpty(i.assetBundleName))
                .ToList()
                .ForEach(i => i.assetBundleName = string.Empty);

            dir.GetDirectories().ToList().ForEach(d => ClearBundleNames(d.FullName));
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Build bundles to output path dir
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="target"></param>
        public static void BuildBundles(string outputPath, BuildTarget target)
        {
            Files.EnsureDir(outputPath);
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, target);
        }

    }
}
