using System.Collections.Generic;
using System.Diagnostics;

namespace Au
{
    public static class Shell
    {
        public static int Run(string filename, List<string> arguments, string workingDir = null)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
#if UNITY_EDITOR_WIN
                FileName = "cmd",
#else
                FileName = "/bin/bash",
#endif
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

#if UNITY_EDITOR_WIN
            startInfo.ArgumentList.Add("/c");
#endif
            startInfo.ArgumentList.Add(filename);
            arguments.ForEach(i => startInfo.ArgumentList.Add(i));

            if (!string.IsNullOrEmpty(workingDir))
            {
                startInfo.WorkingDirectory = workingDir;
            }

            var proc = Process.Start(startInfo);

            proc.WaitForExit();

            if (proc.ExitCode != 0)
            {
                UnityEngine.Debug.LogError(proc.StandardOutput.ReadToEnd());
                UnityEngine.Debug.LogError(proc.StandardError.ReadToEnd());
            }

            return proc.ExitCode;
        }

    }
}
