using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

using Shift.Constant;

namespace Shift.Common.File
{
    [SuppressMessage("NDepend", "ND3101:DontUseSystemRandomForSecurityPurposes", Scope = "method", Justification = "Random number generation is not security-sensitive here, therefore weak psuedo-random numbers are permitted.")]
    public static class PipeHelper
    {
        public delegate Task PipeAction(string path, CancellationToken token);

        private static readonly Random _nameRandom = new Random();
        private static readonly HashSet<string> _pipeNames = new HashSet<string>();
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(5, 5);

        public static void CreatePipe(string toolName, int timeoutMs, Stream stream, PipeAction action) =>
            CreatePipe(toolName, null, timeoutMs, stream, action);

        public static void CreatePipe(string toolName, string fileName, int millisecondsTimeout, Stream stream, PipeAction action)
        {
            var fileExt = Path.GetExtension(fileName.EmptyIfNull());
            var pipeName = GetPipeName(toolName, fileExt);
            var pipePath = GetPipePath(pipeName);

            try
            {
                _lock.Wait();

                using (var tokenSource = new CancellationTokenSource(millisecondsTimeout))
                {
                    var token = tokenSource.Token;

                    token.ThrowIfCancellationRequested();

                    var mainTask = Task.Run(async () =>
                    {
                        using (var pipe = new NamedPipeServerStream(pipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                        {
                            var connectionTask = pipe.WaitForConnectionAsync(token);
                            var actionTask = action(pipePath, token);

                            var completedTask = await Task.WhenAny(connectionTask, actionTask).ConfigureAwait(false);
                            if (completedTask == connectionTask)
                            {
                                try
                                {
                                    await stream.CopyToAsync(pipe, 4096, token).ConfigureAwait(false);
                                }
                                catch (IOException ioex)
                                {
                                    if (ioex.Message != "Pipe is broken.")
                                        throw ioex;
                                }
                                finally
                                {
                                    if (pipe.IsConnected)
                                        pipe.Disconnect();
                                }
                            }

                            await actionTask.ConfigureAwait(false);
                        }
                    }, token);

                    mainTask.Wait();
                }
            }
            finally
            {
                _lock.Release();

                RemovePipeName(pipeName);
            }
        }

        private static string GetPipeName(string tool, string ext)
        {
            lock (_pipeNames)
            {
                for (var i = 0; i < 10; i++)
                {
                    var name = tool + "_" + RandomStringGenerator.Create(_nameRandom, RandomStringType.Alphanumeric, 4) + ext;
                    if (!_pipeNames.Contains(name))
                    {
                        _pipeNames.Add(name);
                        return name;
                    }
                }
            }

            throw ApplicationError.Create("Can't create unique pipe name");
        }

        private static void RemovePipeName(string name)
        {
            lock (_pipeNames)
                _pipeNames.Remove(name);
        }

        private static string GetPipePath(string name)
        {
            return $"\\\\.\\pipe\\{name}";
        }
    }
}
