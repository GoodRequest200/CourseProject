using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using CourseProject.Core.Abstractions;

namespace CourseProject.Api.Services
{
    public class PasswordService : IPasswordService
    {
        private const int SaltSize = 16; // 128-bit salt
        private const int HashLength = 32; // 256-bit hash
        private const int Iterations = 4;
        private const int MemorySize = 1024 * 64; // 64MB
        private const int DegreeOfParallelism = 2;

        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = HashInternal(password, salt);

            // format: base64(salt):base64(hash)
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public bool Verify(string password, string hash)
        {
            var parts = hash.Split(':', 2);
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var expectedHash = Convert.FromBase64String(parts[1]);

            var actualHash = HashInternal(password, salt);

            return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
        }

        private static byte[] HashInternal(string password, byte[] salt)
        {
            var argon = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                Iterations = Iterations,
                MemorySize = MemorySize,
                DegreeOfParallelism = DegreeOfParallelism
            };

            return argon.GetBytes(HashLength);
        }
    }
}
