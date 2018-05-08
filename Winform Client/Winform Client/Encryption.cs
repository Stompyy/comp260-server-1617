using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
namespace Winform_Client
{
    class Encryption
    {
        // code found at https://codereview.stackexchange.com/questions/93614/salt-generation-in-c?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
        private static int saltLengthLimit = 32;
        private static byte[] GetSalt()
        {
            return GetSalt(saltLengthLimit);
        }
        private static byte[] GetSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (rng)
            {
                rng.GetNonZeroBytes(salt);
            }

            return salt;
        }

        /*
         * (from same source as code above)
         * 
         * If you are going to call getSalt() more than once while your application is running, you should keep a single static instance of RNGCryptoServiceProvider 
         * in your class instead of creating a new one in every invocation of getSalt(). This is because the 0-argument RNGCryptoServiceProvider constructor is not 
         * guaranteed to get its entropy from a cryptographic-quality source. You could end up with predictable sequences of salts or even repetitions of the same salt.
         */

        static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public static String encryptPasswordWithSalt(String password, out String saltString)
        {
            var salt = GetSalt();
            rng.GetBytes(salt);
            var hash = GenerateSaltedHash(Encoding.UTF8.GetBytes(password), salt);

            saltString = Convert.ToBase64String(salt);

            return Convert.ToBase64String(hash);
        }

        public static String encryptPasswordWithSalt(String password, Byte[] salt)
        {
            var hash = GenerateSaltedHash(Encoding.UTF8.GetBytes(password), salt);

            return Convert.ToBase64String(hash);
        }

        static byte[] GenerateHash(byte[] plainText)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            return algorithm.ComputeHash(plainText);
        }

        static byte[] GenerateSaltedHash(byte[] plainTextPassword, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes = new byte[plainTextPassword.Length + salt.Length];

            for (int i = 0; i < plainTextPassword.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainTextPassword[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainTextPassword.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }
    }
}
