using System;
using System.Diagnostics;
using System.IO;

namespace Shift.Common
{
    public class ProcessHelper
    {
        public static string InitializeLogging(string folder)
        {
            if (string.IsNullOrEmpty(folder))
                return null;

            var appName = GetExecutingApplicationName();

            var appFolder = Path.Combine(folder, appName)?.ToLower();

            if (appFolder == null)
                return null;

            Directory.CreateDirectory(appFolder);

            var filePath = Path.Combine(appFolder, "serilog-.log");

            return filePath;
        }

        public static string InitializeMonitoring(string folder)
        {
            if (string.IsNullOrEmpty(folder))
                return null;

            var appName = GetExecutingApplicationName();

            var appFolder = Path.Combine(folder, appName)?.ToLower();

            if (appFolder == null)
                return null;

            Directory.CreateDirectory(appFolder);

            var filePath = Path.Combine(appFolder, "sentry.log");

            return filePath;
        }

        /// <summary>
        /// Returns the full executable path and name
        /// </summary>
        /// <returns>The full path to the executing application</returns>
        public static string GetExecutingApplicationPath()
        {
            return Process.GetCurrentProcess().MainModule?.FileName ??
                   System.Reflection.Assembly.GetEntryAssembly()?.Location ??
                   "Unknown";
        }

        /// <summary>
        /// Returns the name of the executing application (process name without extension)
        /// </summary>
        /// <remarks>
        /// This may differ from the assembly name in cases where the executable has been renamed or when running under 
        /// certain hosting environments.
        /// </remarks>
        /// <returns>The name of the executing application</returns>
        public static string GetExecutingApplicationName()
        {
            try
            {
                // Get the current process name (without .exe extension)
                return Process.GetCurrentProcess().ProcessName;
            }
            catch (Exception ex)
            {
                // Fallback: use the executable file name if process name fails
                try
                {
                    string executablePath = Process.GetCurrentProcess().MainModule?.FileName;
                    if (!string.IsNullOrEmpty(executablePath))
                    {
                        return Path.GetFileNameWithoutExtension(executablePath);
                    }
                }
                catch
                {
                    // If all else fails, return the entry assembly name
                    return System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
                }

                throw new InvalidOperationException("Unable to determine application name", ex);
            }
        }
    }
}
