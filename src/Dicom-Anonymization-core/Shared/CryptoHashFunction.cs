
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace De_Id_Function_Shared
{
    public class CryptoHashFunction
    {
        public static byte[] ComputeHmacSHA256Hash(byte[] input, byte[] hashKey = null)
        {
            if (input == null)
            {
                return input;
            }

            HMAC hmac = new HMACSHA256();
            if (hashKey != null)
            {
                hmac = new HMACSHA256(hashKey);
            }

            return hmac.ComputeHash(input);
        }

        public static byte[] ComputeHmacSHA256Hash(Stream input, byte[] hashKey = null)
        {
            if (input == null)
            {
                return null;
            }

            HMAC hmac = new HMACSHA256();
            if (hashKey != null)
            {
                hmac = new HMACSHA256(hashKey);
            }

            return hmac.ComputeHash(input);
        }

        public static byte[] ComputeHmacSHA256Hash(string input, byte[] hashKey = null)
        {
            if (input == null)
            {
                return null;
            }


            var plainData = Encoding.UTF8.GetBytes(input);
            return ComputeHmacSHA256Hash(plainData, hashKey);
        }

        public static FixedLengthString ComputeHmacSHA256Hash(FixedLengthString input, byte[] hashKey = null)
        {
            if (input == null)
            {
                return input;
            }

            var hashData = ComputeHmacSHA256Hash(input.ToString(), hashKey);

            return new FixedLengthString(input.GetLength(), string.Concat(hashData.Select(b => b.ToString("x2"))));
        }

        public static byte[] ComputeHmacHash(byte[] input, HMAC hashAlgorithm)
        {
            if (input == null)
            {
                return null;
            }

            return hashAlgorithm.ComputeHash(input);
        }

        public static byte[] ComputeHmacHash(string input, HMAC hashAlgorithm)
        {
            if (input == null)
            {
                return null;
            }

            var plainData = Encoding.UTF8.GetBytes(input);
            return hashAlgorithm.ComputeHash(plainData);
        }

        public static byte[] ComputeHmacHash(Stream input, HMAC hashAlgorithm)
        {
            if (input == null)
            {
                return null;
            }

            return hashAlgorithm.ComputeHash(input);
        }

        public static FixedLengthString ComputeHmacHash(FixedLengthString input, HMAC hashAlgorithm)
        {
            if (input == null)
            {
                return input;
            }

            var hashData = ComputeHmacHash(input.ToString(), hashAlgorithm);

            return new FixedLengthString(input.GetLength(), string.Concat(hashData.Select(b => b.ToString("x2"))));
        }


    }
}
