using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace instantMessagingCore.Crypto
{
    public class PasswordUtils
    {
        private static UTF8Encoding encoding = new UTF8Encoding();

        /// <summary>
        /// Hash and salt the password
        /// </summary>
        /// <param name="password">the original password</param>
        /// <param name="salt">the salt to used</param>
        /// <returns>a salted and hashed password</returns>
        public static string hashAndSalt(string password, string salt) => sha256(password + salt);

        /// <summary>
        /// Hash a value
        /// </summary>
        /// <param name="value">the value to hash</param>
        /// <returns>The value hashed</returns>
        public static string sha256(string value)
        {
            using (SHA256 hash = SHA256.Create())
            {
                byte[] bytes = hash.ComputeHash(encoding.GetBytes(value));
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }

        /// <summary>
        /// Generate a salt for the password
        /// </summary>
        /// <returns>A new salt</returns>
        public static string getSalt()
        {
            byte[] bytes = new byte[128 / 8];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }

        private static readonly char[] alphabetLower = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static readonly char[] alphabetUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] numbers = "0123456789".ToCharArray();
        private static readonly char[] symboles = "&é\"'(-è_çà)=^$ù*<,;:!~#{[|`\\^@]}¤¨£%µ>?./§".ToCharArray();

        /// <summary>
        /// Check if the password is compliant
        /// </summary>
        /// <param name="username">the username to avoid in password</param>
        /// <param name="password">the password to check</param>
        /// <returns>true if the password is compliant</returns>
        public static bool CheckPolicy(String username, String password)
        {
            bool result = password.Length >= 8 &&
                password.Length <= 255 &&
                !password.Contains(username) &&
                password.Any(c => alphabetLower.Contains(c)) &&
                password.Any(c => alphabetUpper.Contains(c)) &&
                password.Any(c => numbers.Contains(c)) &&
                password.Any(c => symboles.Contains(c));

            username = null;
            password = null;

            return result;
        }

    }
}
