using System;
using System.IO;

namespace Shift.Common
{
    public static class CryptoHelper
    {
        #region stream encryption
        public static void EncryptStream(string password, string salt, Stream inputStream, Stream outputStream, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            if (!inputStream.CanRead)
                throw new Exception("The input stream has to support read");
            if (!outputStream.CanWrite)
                throw new Exception("The output stream has to support write");

            EncryptionBuffer encBuffer = new EncryptionBuffer(password, salt, algorithm);
            byte[] readBuffer = new byte[500000];
            bool isLastData = false;
            while (!isLastData)
            {
                int nrOfBytes = inputStream.Read(readBuffer, 0, readBuffer.Length);
                isLastData = (nrOfBytes == 0);

                encBuffer.AddData(readBuffer, 0, nrOfBytes, isLastData);
                byte[] encryptedData = encBuffer.GetData();
                outputStream.Write(encryptedData, 0, encryptedData.Length);
            }
        }

        public static void DecryptStream(string password, string salt, Stream inputStream, Stream outputStream, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            if (!inputStream.CanRead)
                throw new Exception("The input stream has to support read");
            if (!outputStream.CanWrite)
                throw new Exception("The output stream has to support write");

            DecryptionBuffer decBuffer = new DecryptionBuffer(password, salt, algorithm);

            DecryptStream(inputStream, outputStream, decBuffer);
        }

        public static void DecryptStreamWithoutSalt(byte[] ivBytes, byte[] keyBytes, Stream inputStream, Stream outputStream, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            if (!inputStream.CanRead)
                throw new Exception("The input stream has to support read");
            if (!outputStream.CanWrite)
                throw new Exception("The output stream has to support write");

            DecryptionBuffer decBuffer = DecryptionBuffer.CreateWithoutSalt(ivBytes, keyBytes, algorithm);

            DecryptStream(inputStream, outputStream, decBuffer);
        }

        private static void DecryptStream(Stream inputStream, Stream outputStream, DecryptionBuffer decBuffer)
        {
            byte[] readBuffer = new byte[500000];
            bool isLastData = false;
            while (!isLastData)
            {
                int nrOfBytes = inputStream.Read(readBuffer, 0, readBuffer.Length);
                isLastData = (nrOfBytes == 0);

                decBuffer.AddData(readBuffer, 0, nrOfBytes, isLastData);
                byte[] decryptedData = decBuffer.GetData();
                outputStream.Write(decryptedData, 0, decryptedData.Length);
            }
        }

        public static void EncryptStream(string password, AutoSaltSizes saltSize, Stream inputStream, Stream outputStream, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            if (!inputStream.CanRead)
                throw new Exception("The input stream has to support read");
            if (!outputStream.CanWrite)
                throw new Exception("The output stream has to support write");

            EncryptionBuffer encBuffer = new EncryptionBuffer(password, saltSize, algorithm);
            byte[] readBuffer = new byte[500000];
            bool isLastData = false;
            while (!isLastData)
            {
                int nrOfBytes = inputStream.Read(readBuffer, 0, readBuffer.Length);
                isLastData = (nrOfBytes == 0);

                encBuffer.AddData(readBuffer, 0, nrOfBytes, isLastData);
                byte[] encryptedData = encBuffer.GetData();
                outputStream.Write(encryptedData, 0, encryptedData.Length);
            }
        }

        public static void DecryptStream(string password, AutoSaltSizes saltSize, Stream inputStream, Stream outputStream, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            if (!inputStream.CanRead)
                throw new Exception("The input stream has to support read");
            if (!outputStream.CanWrite)
                throw new Exception("The output stream has to support write");

            DecryptionBuffer decBuffer = new DecryptionBuffer(password, saltSize, algorithm);
            byte[] readBuffer = new byte[500000];
            bool isLastData = false;
            while (!isLastData)
            {
                int nrOfBytes = inputStream.Read(readBuffer, 0, readBuffer.Length);
                isLastData = nrOfBytes == 0;

                decBuffer.AddData(readBuffer, 0, nrOfBytes, isLastData);
                byte[] decryptedData = decBuffer.GetData();
                outputStream.Write(decryptedData, 0, decryptedData.Length);
            }
        }
        #endregion

        #region file encryption
        public static void EncryptFile(string password, string salt, string inputFileName, string outputFileName, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            using (FileStream inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            {
                using (FileStream outputStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
                {
                    EncryptStream(password, salt, inputStream, outputStream, algorithm);
                }
            }
        }

        public static void DecryptFile(string password, string salt, string inputFileName, string outputFileName, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            using (FileStream inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            {
                using (FileStream outputStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
                {
                    DecryptStream(password, salt, inputStream, outputStream, algorithm);
                }
            }
        }

        public static void EncryptFile(string password, AutoSaltSizes saltSize, string inputFileName, string outputFileName, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            using (FileStream inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            {
                using (FileStream outputStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
                {
                    EncryptStream(password, saltSize, inputStream, outputStream, algorithm);
                }
            }
        }

        public static void DecryptFile(string password, AutoSaltSizes saltSize, string inputFileName, string outputFileName, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            using (FileStream inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            {
                using (FileStream outputStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
                {
                    DecryptStream(password, saltSize, inputStream, outputStream, algorithm);
                }
            }
        }
        #endregion

        #region data encryption
        public static byte[] EncryptData(string password, string salt, byte[] data, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            EncryptionBuffer encBuffer = new EncryptionBuffer(password, salt, algorithm);
            encBuffer.AddData(data, true);
            return encBuffer.GetData();
        }

        public static byte[] DecryptData(string password, string salt, byte[] data, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            DecryptionBuffer decBuffer = new DecryptionBuffer(password, salt, algorithm);
            decBuffer.AddData(data, true);
            return decBuffer.GetData();
        }

        public static byte[] EncryptData(string password, AutoSaltSizes saltSize, byte[] data, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            EncryptionBuffer encBuffer = new EncryptionBuffer(password, saltSize, algorithm);
            encBuffer.AddData(data, true);
            return encBuffer.GetData();
        }

        public static byte[] DecryptData(string password, AutoSaltSizes saltSize, byte[] data, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            DecryptionBuffer decBuffer = new DecryptionBuffer(password, saltSize, algorithm);
            decBuffer.AddData(data, true);
            return decBuffer.GetData();
        }
        #endregion

        #region string encryption
        public static string EncryptString(string password, string salt, string strData, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            byte[] data = EncryptionHelper.GetBytes(strData);
            byte[] encryptedData = EncryptData(password, salt, data, algorithm);
            return EncryptionHelper.GetString(encryptedData);
        }

        public static string DecryptString(string password, string salt, string strData, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            byte[] data = EncryptionHelper.GetBytes(strData);
            byte[] encryptedData = DecryptData(password, salt, data, algorithm);
            return EncryptionHelper.GetString(encryptedData);
        }

        public static string EncryptString(string password, AutoSaltSizes saltSize, string strData, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            byte[] data = EncryptionHelper.GetBytes(strData);
            byte[] encryptedData = EncryptData(password, saltSize, data, algorithm);
            return EncryptionHelper.GetString(encryptedData);
        }

        public static string DecryptString(string password, AutoSaltSizes saltSize, string strData, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            byte[] data = EncryptionHelper.GetBytes(strData);
            byte[] encryptedData = DecryptData(password, saltSize, data, algorithm);
            return EncryptionHelper.GetString(encryptedData);
        }
        #endregion

        #region Base 64 string encryption
        public static string EncryptBase64String(string password, string salt, string strData, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            byte[] data = EncryptionHelper.GetBytes(strData);
            byte[] encryptedData = EncryptData(password, salt, data, algorithm);
            return Convert.ToBase64String(encryptedData);
        }

        public static string DecryptBase64String(string password, string salt, string b64StrData, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            byte[] data = Convert.FromBase64String(b64StrData);
            byte[] encryptedData = DecryptData(password, salt, data, algorithm);
            return EncryptionHelper.GetString(encryptedData);
        }

        public static string EncryptBase64String(string password, AutoSaltSizes saltSize, string strData, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            byte[] data = EncryptionHelper.GetBytes(strData);
            byte[] encryptedData = EncryptData(password, saltSize, data, algorithm);
            return Convert.ToBase64String(encryptedData);
        }

        public static string DecryptBase64String(string password, AutoSaltSizes saltSize, string b64StrData, SymmetricEncryptionAlgorithm algorithm = SymmetricEncryptionAlgorithm.AES_256_CBC)
        {
            byte[] data = Convert.FromBase64String(b64StrData);
            byte[] encryptedData = DecryptData(password, saltSize, data, algorithm);
            return EncryptionHelper.GetString(encryptedData);
        }
        #endregion

        #region Password encryption

        public static string EncryptPassword(string password, string secretKeyPath)
        {
            if (!string.IsNullOrEmpty(password) && System.IO.File.Exists(secretKeyPath))
            {
                var secret = System.IO.File.ReadAllText(secretKeyPath);
                return EncryptBase64String(secret, AutoSaltSizes.Salt64, password);
            }

            return null;
        }

        public static string DecryptPassword(string cipher, string secretKeyPath)
        {
            if (!string.IsNullOrEmpty(cipher) && System.IO.File.Exists(secretKeyPath))
            {
                var secret = System.IO.File.ReadAllText(secretKeyPath);
                return DecryptBase64String(secret, AutoSaltSizes.Salt64, cipher);
            }

            return null;
        }

        #endregion
    }
}
