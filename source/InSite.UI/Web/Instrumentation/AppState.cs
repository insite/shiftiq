using System.Threading;
using System.Threading.Tasks;

using Shift.Constant;

namespace InSite.Common
{
    public static class AppState
    {
        #region Classes

        private static class MaintenanceMode
        {
            public const string Enabled = "Enabled";
            public const string Disabled = "Disabled";
        }

        #endregion

        #region Fields

        private static bool _isAutomationRunning = false;
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        #endregion

        #region Methods

        public static async Task<bool> TryStartAutomationAsync()
        {
            try
            {
                await _lock.WaitAsync();

                if (_isAutomationRunning || IsMaintenanceModeEnabledInternal())
                    return false;

                _isAutomationRunning = true;

                return true;
            }
            finally
            {
                _lock.Release();
            }
        }

        public static async Task<bool> TryStopAutomationAsync()
        {
            try
            {
                await _lock.WaitAsync();

                if (!_isAutomationRunning)
                    return false;

                _isAutomationRunning = false;

                return true;
            }
            finally
            {
                _lock.Release();
            }
        }

        public static async Task<AutomationStateType> GetAutomationStateAsync()
        {
            try
            {
                await _lock.WaitAsync();

                return GetAutomationStateInternal();
            }
            finally
            {
                _lock.Release();
            }
        }

        public static AutomationStateType GetAutomationState()
        {
            try
            {
                _lock.Wait();

                return GetAutomationStateInternal();
            }
            finally
            {
                _lock.Release();
            }
        }

        private static AutomationStateType GetAutomationStateInternal()
        {
            if (_isAutomationRunning)
                return AutomationStateType.Running;

            if (IsMaintenanceModeEnabledInternal())
                return AutomationStateType.Maintenance;

            return AutomationStateType.Idle;
        }

        public static async Task<bool> TryEnableMaintenanceModeAsync(bool enable)
        {
            try
            {
                await _lock.WaitAsync();

                return TryEnableMaintenanceModeInternal(enable);
            }
            finally
            {
                _lock.Release();
            }
        }

        public static bool TryEnableMaintenanceMode(bool enable)
        {
            try
            {
                _lock.Wait();

                return TryEnableMaintenanceModeInternal(enable);
            }
            finally
            {
                _lock.Release();
            }
        }

        private static bool TryEnableMaintenanceModeInternal(bool enable)
        {
            if (_isAutomationRunning)
                return false;

            ServiceLocator.Maintenance.Mode = enable;

            return true;
        }

        public static async Task<bool> IsMaintenanceModeEnabledAsync()
        {
            try
            {
                await _lock.WaitAsync();

                return IsMaintenanceModeEnabledInternal();
            }
            finally
            {
                _lock.Release();
            }
        }

        public static bool IsMaintenanceModeEnabled()
        {
            try
            {
                _lock.Wait();

                return IsMaintenanceModeEnabledInternal();
            }
            finally
            {
                _lock.Release();
            }
        }

        private static bool IsMaintenanceModeEnabledInternal()
        {
            return !_isAutomationRunning && ServiceLocator.Maintenance.Mode;
        }

        #endregion
    }
}
