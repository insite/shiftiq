using System;
using System.Diagnostics;
using System.IO;

namespace Shift.Common
{
    public static class PandocFileConverter
    {
        public static string Convert(PandocConverterSettings settings)
        {
            return Execute(settings, process =>
            {

            });
        }

        private static string Execute(PandocConverterSettings settings, Action<Process> setupProcess = null)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!System.IO.File.Exists(settings.ExePath))
                throw new PandocConverterException($"Pandoc.exe not found in: {settings.ExePath}");

            string error, result;

            var psi = new ProcessStartInfo
            {
                FileName = settings.ExePath,
                Arguments = settings.GetArgs(),

                UseShellExecute = false,
                CreateNoWindow = true,

                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            try
            {
                using (var process = Process.Start(psi))
                {
                    if (process == null)
                        throw new PandocConverterException("Pandoc conversion failed:\r\n    - Process failed to start");

                    setupProcess?.Invoke(process);

                    using (var ms = new MemoryStream())
                    {
                        using (var stdout = process.StandardOutput)
                            stdout.BaseStream.CopyTo(ms);
                        result = ms.ToString();
                    }

                    process.WaitForExit(10000);

                    error = process.StandardError.ReadToEnd();

                    process.WaitForExit(10000);
                }
            }
            catch (Exception ex)
            {
                throw new PandocConverterException($"Conversion failed:\r\n    - Settings: {settings.ReadSettings()}", ex);
            }

            if (result.IsEmpty() && error.IsNotEmpty())
                throw new PandocConverterException($"Conversion failed: {error}\r\n    - Settings: {settings.ReadSettings()}\r\n");

            return settings.OutputFilePath;
        }
    }
}
