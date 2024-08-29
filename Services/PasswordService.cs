using System;
using System.Security.Cryptography;
using System.Text;

namespace AppShowDoMilhao.Services
{
    public static class PasswordService
    {
        public static byte[] GenerateSalt()
        {
            var saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return saltBytes;
        }

        public static byte[] HashPassword(string password, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
