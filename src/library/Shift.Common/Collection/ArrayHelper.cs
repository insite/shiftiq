using System;
using System.Linq;

namespace Shift.Common
{
    public static class ArrayHelper
    {
        public static byte[] Combine(byte[] a1, byte[] a2)
        {
            var result = new byte[a1.Length + a2.Length];

            Buffer.BlockCopy(a1, 0, result, 0, a1.Length);
            Buffer.BlockCopy(a2, 0, result, a1.Length, a2.Length);

            return result;
        }

        public static byte[] Combine(byte[] a1, byte[] a2, byte[] a3)
        {
            var result = new byte[a1.Length + a2.Length + a3.Length];

            Buffer.BlockCopy(a1, 0, result, 0, a1.Length);
            Buffer.BlockCopy(a2, 0, result, a1.Length, a2.Length);
            Buffer.BlockCopy(a3, 0, result, a1.Length + a2.Length, a3.Length);

            return result;
        }

        public static byte[] Combine(byte[] a1, byte[] a2, byte[] a3, byte[] a4)
        {
            var result = new byte[a1.Length + a2.Length + a3.Length + a4.Length];

            Buffer.BlockCopy(a1, 0, result, 0, a1.Length);
            Buffer.BlockCopy(a2, 0, result, a1.Length, a2.Length);
            Buffer.BlockCopy(a3, 0, result, a1.Length + a2.Length, a3.Length);
            Buffer.BlockCopy(a4, 0, result, a1.Length + a2.Length + a3.Length, a4.Length);

            return result;
        }

        public static byte[] Combine(params byte[][] arrays)
        {
            var result = new byte[arrays.Sum(x => x.Length)];

            var offset = 0;
            foreach (byte[] data in arrays)
            {
                Buffer.BlockCopy(data, 0, result, offset, data.Length);
                offset += data.Length;
            }

            return result;
        }

        public static byte[] Get(byte[] array, int offset, int length)
        {
            var result = new byte[length];
            Buffer.BlockCopy(array, offset, result, 0, result.Length);
            return result;
        }
    }
}
