using System;
using System.Security.Cryptography;
using System.Text;


namespace hotel_management.Services.Auth
{
    public static class PasswordHasher
    {
        public static string GenerateSalt()
        {
            var saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string Hash(string password, string salt)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password + salt);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static string HashPassword(string password)
        {
            var salt = GenerateSalt();
            var hash = Hash(password, salt);
            return $"{salt}:{hash}";
        }

        public static bool Verify(string password, string stored)
        {
            try
            {
                var parts = stored.Split(':');
                if (parts.Length != 2) return false;

                var salt = parts[0];
                var hash = parts[1];

                var hashOfInput = Hash(password, salt);
                return hashOfInput == hash;
            }
            catch
            {
                return false;
            }
        }
    }
}
