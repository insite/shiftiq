using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Shift.Common
{
    internal class AsyncProcess : IDisposable
    {
        #region Properties

        public bool HasExited => _process.HasExited;

        public int ExitCode => _process.ExitCode;

        public ReadOnlyCollection<string> StandardOutput => _stdout.AsReadOnly();

        public ReadOnlyCollection<string> StandardError => _stderr.AsReadOnly();

        #endregion

        #region Fields

        private Process _process;

        private List<string> _stdout;
        private List<string> _stderr;
        private Action<string> _stdoutCallback;

        private CancellationTokenRegistration _cancelRegistration;
        private TaskCompletionSource<bool> _exitTask;
        private TaskCompletionSource<bool> _stdoutTask;
        private TaskCompletionSource<bool> _stderrTask;

        #endregion

        #region Construction

        private AsyncProcess(ProcessStartInfo info)
        {
            _process = new Process
            {
                StartInfo = info,
                EnableRaisingEvents = true
            };

            _stdout = new List<string>();
            _stderr = new List<string>();
            _exitTask = new TaskCompletionSource<bool>();
            _stdoutTask = new TaskCompletionSource<bool>();
            _stderrTask = new TaskCompletionSource<bool>();
        }

        private void Start(Action<string> stdoutCallback, CancellationToken token)
        {
            if (stdoutCallback != null)
            {
                _stdoutCallback = stdoutCallback;
                _process.OutputDataReceived += OnOutputDataReceivedCallback;
            }
            else
            {
                _process.OutputDataReceived += OnOutputDataReceivedDefault;
            }

            _process.ErrorDataReceived += OnErrorDataReceived;
            _process.Exited += OnExited;

            _cancelRegistration = token.Register(OnProcessCancelled, _process);

            if (!_process.Start())
                throw ApplicationError.Create("Can't start the process");

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        #endregion

        #region Public methods

        public static AsyncProcess Start(ProcessStartInfo info, CancellationToken token) =>
            Start(info, null, token);

        public static AsyncProcess Start(ProcessStartInfo info, Action<string> stdoutCallback, CancellationToken token)
        {
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            var instance = new AsyncProcess(info);

            instance.Start(stdoutCallback, token);

            return instance;
        }

        public async Task WaitForExitAsync()
        {
            await _exitTask.Task.ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_cancelRegistration != default)
                _cancelRegistration.Dispose();

            _process?.Dispose();
        }

        #endregion

        #region Event handlers

        private void OnOutputDataReceivedDefault(object sender, DataReceivedEventArgs e)
        {
            OnDataReceived(_stdout, e.Data, _stdoutTask);
        }

        private void OnOutputDataReceivedCallback(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                _stdoutCallback(e.Data);
            else
                _stdoutTask.TrySetResult(true);
        }

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnDataReceived(_stderr, e.Data, _stderrTask);
        }

        private static void OnDataReceived(List<string> list, string data, TaskCompletionSource<bool> task)
        {
            if (data != null)
                list.Add(data);
            else
                task.TrySetResult(true);
        }

        private void OnExited(object sender, EventArgs e)
        {
            Task.WhenAll(_stdoutTask.Task, _stderrTask.Task).ContinueWith(_ =>
            {
                return _exitTask.TrySetResult(result: true);
            });
        }

        private static void OnProcessCancelled(object state)
        {
            var process = (Process)state;

            try
            {
                if (process != null && !process.HasExited)
                    process.Kill();
            }
            catch (InvalidOperationException)
            {

            }
        }

        #endregion
    }
}
