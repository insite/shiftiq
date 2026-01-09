using System;
using System.Diagnostics;

namespace Shift.Common
{
    public static class RemoteShare
    {
        public static void AddShares(RemoteShareItem[] items)
        {
            foreach (var item in items)
            {
                var arguments = $"/add:\"{item.Target}\" /user:\"{item.User}\" /pass:\"{item.Password}\"";
                RunCmdKey(arguments);
            }
        }

        public static void DeleteShares(RemoteShareItem[] items)
        {
            foreach (var item in items)
            {
                var arguments = $"/delete:\"{item.Target}\"";
                RunCmdKey(arguments);
            }
        }

        private static void RunCmdKey(string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmdkey",
                WorkingDirectory = null,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = arguments,
            };

            string error;

            using (var process = Process.Start(psi))
            {
                if (process == null)
                    throw new ApplicationException("cmdkey failed:\r\n    - Process failed to start");

                error = process.StandardError.ReadToEnd();

                process.WaitForExit(10000);
            }

            if (!string.IsNullOrEmpty(error))
                throw new ApplicationException($"cmdkey failed: {error}");
        }
    }
}
