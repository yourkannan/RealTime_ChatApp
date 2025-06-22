using CHAT.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace CHAT.Helpers
{
    public static class PasswordHelper
    {
        private static PasswordHasher<User> Hasher = new();

        public static string Hash(User u, string password) => Hasher.HashPassword(u, password);

        public static bool Verify(User u, string password) =>
            Hasher.VerifyHashedPassword(u, u.PasswordHash, password) != PasswordVerificationResult.Failed;
    }
}