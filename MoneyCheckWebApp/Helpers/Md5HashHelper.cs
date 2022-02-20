using System;
using System.Security.Cryptography;
using System.Text;

namespace MoneyCheckWebApp.Helpers
{
    public class Md5HashHelper : IDisposable
    {
        private readonly MD5 _hasher;

        ~Md5HashHelper()
        {
            Dispose();
        }

        public Md5HashHelper()
        {
            _hasher = MD5.Create();
        }
        
        public string Compute(string @string)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(@string);
            var hash = _hasher.ComputeHash(passwordBytes);

            var encoded = BitConverter.ToString(hash)
                .Replace("-", string.Empty)
                .ToLower();

            return encoded;
        }

        public void Dispose()
        {
            _hasher.Dispose();
        }
    }
}