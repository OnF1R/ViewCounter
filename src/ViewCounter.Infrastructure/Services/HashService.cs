using System.Security.Cryptography;
using System.Text;
using ViewCounter.Application.Abstractions.Services;

namespace ViewCounter.Infrastructure.Services
{
    public class HashService : IHashService
    {
        public string Hash(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
