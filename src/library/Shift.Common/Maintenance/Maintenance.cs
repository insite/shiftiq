using System;

namespace Shift.Common
{
    public class Maintenance
    {
        private const string MaintenanceFileName = "MaintenanceMode.txt";

        private readonly string _maintenanceFilePath;

        public bool _mode;
        public bool Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
                SetMode(value);
            }
        }

        public Maintenance(FilePaths filePaths)
        {
            _maintenanceFilePath = filePaths.GetPhysicalPathToTempFile(MaintenanceFileName);
            _mode = GetMode();
        }

        private bool GetMode()
        {
            if (!System.IO.File.Exists(_maintenanceFilePath))
                return false;

            var value = System.IO.File.ReadAllText(_maintenanceFilePath).Trim();

            if (bool.TryParse(value, out bool result))
                return result;

            throw new Exception("Unexpected Boolean Format: " + value);
        }

        private void SetMode(bool value)
        {
            if (!value)
            {
                if (System.IO.File.Exists(_maintenanceFilePath))
                    System.IO.File.Delete(_maintenanceFilePath);
            }
            else
            {
                System.IO.File.WriteAllText(_maintenanceFilePath, value.ToString());
            }
        }
    }
}
