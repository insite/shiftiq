using IoFile = System.IO.File;

namespace Shift.Common
{
    public class PermissionRefreshExchange
    {
        private readonly string _permissionRefreshFilePath;

        public PermissionRefreshExchange(string permissionRefreshFilePath)
        {
            _permissionRefreshFilePath = permissionRefreshFilePath;
        }

        public void MarkRefreshed()
        {
            IoFile.WriteAllText(_permissionRefreshFilePath, "Refreshed");
        }

        public void RemoveMark()
        {
            IoFile.WriteAllText(_permissionRefreshFilePath, "");
        }

        public bool IsRefreshed()
        {
            return IoFile.Exists(_permissionRefreshFilePath)
                && IoFile.ReadAllText(_permissionRefreshFilePath) == "Refreshed";
        }
    }
}