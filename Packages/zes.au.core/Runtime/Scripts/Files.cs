using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Au
{
    /// <summary>
    /// Files helper library
    /// </summary>
    public static class Files
    {
        readonly static string[] webprefix = new string[] { "jar:", "http:", "https:", "file:" };

        /// <summary>
        /// UTF8 Encoding without BOM
        /// </summary>
        public readonly static Encoding utf8WithoutBOM = new UTF8Encoding(false);

        /// <summary>
        /// Tell if a path present a file from web
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsWebFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            path = path.ToLower();

            foreach (var item in webprefix)
            {
                if (path.StartsWith(item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Copy file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static async Task<bool> Copy(string source, string dest)
        {
            FileInfo fi = new FileInfo(dest);
            EnsureDir(fi.Directory); // ensure dir
            if (IsWebFile(source))
            {
                var www = UnityWebRequest.Get(source);
                www.downloadHandler = new DownloadHandlerFile(dest);
                var op = www.SendWebRequest();
                await Async.WaitAsyncOperation(op);
            }
            else
            {
                File.Copy(source, dest, true);
            }
            return true;
        }

        /// <summary>
        /// Read string from file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<string> Read(string file)
        {
            byte[] buffer = null;
            if (IsWebFile(file))
            {
                var www = UnityWebRequest.Get(file);
                www.downloadHandler = new DownloadHandlerBuffer();
                var op = www.SendWebRequest();
                await Async.WaitAsyncOperation(op);
                buffer = www.downloadHandler.data;
            }
            else
            {
                buffer = File.ReadAllBytes(file);
            }
            return utf8WithoutBOM.GetString(buffer);
        }

        /// <summary>
        /// Save text file
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="text"></param>
        public static void Save(string dest, string text)
        {
            FileInfo fi = new FileInfo(dest);
            EnsureDir(fi.Directory); // ensure dir
            File.WriteAllText(dest, text, utf8WithoutBOM);
        }

        /// <summary>
        /// Check if file exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Ensuare a dir exists
        /// </summary>
        /// <param name="dir"></param>
        /// <returns>DirectoryInfo of dir</returns>
        public static DirectoryInfo EnsureDir(string dir)
        {
            return EnsureDir(new DirectoryInfo(dir));
        }

        /// <summary>
        /// Ensure a dir exists
        /// </summary>
        /// <param name="dir"></param>
        /// <returns>DirectoryInfo of dir</returns>
        public static DirectoryInfo EnsureDir(DirectoryInfo dir)
        {
            if (!dir.Parent.Exists)
            {
                EnsureDir(dir.Parent);
            }

            if (!dir.Exists)
            {
                dir.Create();
            }

            return dir;
        }

        /// <summary>
        /// Clear dir
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static DirectoryInfo ClearDir(string dir)
        {
            return ClearDir(new DirectoryInfo(dir));
        }

        /// <summary>
        /// Clear dir
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static DirectoryInfo ClearDir(DirectoryInfo dir)
        {
            if (!dir.Exists)
            {
                return dir;
            }
            dir.GetFiles().ToList().ForEach(f => f.Delete());
            dir.GetDirectories().ToList().ForEach(d => d.Delete(true));
            return dir;
        }

        /// <summary>
        /// Copy Dir
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void CopyDir(string from, string to)
        {
            CopyDir(new DirectoryInfo(from), new DirectoryInfo(to));
        }

        /// <summary>
        /// Copy Dir
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void CopyDir(DirectoryInfo from, DirectoryInfo to)
        {
            if (!from.Exists)
            {
                Debug.LogError($"Copy dir failed: {from.FullName} not existed");
                return;
            }

            EnsureDir(to);

            from.GetFiles()
                .ToList()
                .ForEach(file => file.CopyTo(Path.Combine(to.FullName, file.Name), true));

            from.GetDirectories()
                .ToList()
                .ForEach(dir => CopyDir(dir, new DirectoryInfo(Path.Combine(to.FullName, dir.Name))));
        }

    }
}
