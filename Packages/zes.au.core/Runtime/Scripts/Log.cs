using System.Collections.Generic;
using UnityEngine;

namespace Au
{
    /// <summary>
    /// Log Class
    /// </summary>
    public class Log
    {
        private static Dictionary<string, Log> loggers = new Dictionary<string, Log>();

        /// <summary>
        /// Get a Logger from type
        /// </summary>
        /// <typeparam name="T">Class Type</typeparam>
        /// <returns>Logger</returns>
        public static Log GetLogger<T>()
        {
            return GetLogger(typeof(T).FullName);
        }

        /// <summary>
        /// Get a Logger from name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Logger</returns>
        public static Log GetLogger(string name)
        {
            if (loggers.TryGetValue(name, out Log logger))
            {
                return logger;
            }
            logger = new Log(name);
            loggers.Add(name, logger);
            return logger;
        }

        private Log(string name)
        {
            this.name = name;
        }

        private string name;

        public void Info(object message)
        {
            Debug.Log($"[{name}] {message}");
        }

        public void Warn(object message)
        {
            Debug.LogWarning($"[{name}] {message}");
        }

        public void Error(object message)
        {
            Debug.LogError($"[{name}] {message}");
        }
    }
}
