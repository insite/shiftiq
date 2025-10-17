namespace Shift.Common
{
    public static class PasswordHelper
    {
        public static string EncryptPassword(string password, string keyPath)
        {
            string cipher = CryptoHelper.EncryptPassword(password, keyPath);

            if (!string.IsNullOrEmpty(password) && string.IsNullOrEmpty(cipher))
                throw new ApplicationError("Password cannot be encrypted.");

            return cipher;
        }

        public static string DecryptPassword(string cipher, string keyPath)
        {
            string password = CryptoHelper.DecryptPassword(cipher, keyPath);

            if (!string.IsNullOrEmpty(cipher) && string.IsNullOrEmpty(password))
                throw new ApplicationError("Password cannot be decrypted.");

            return password;
        }
    }
}
