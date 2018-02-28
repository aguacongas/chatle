using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatLe.Cryptography
{
    public class Utility : IUtility
    {
        private readonly string _key;
        private readonly SymmetricAlgorithm _algorithm = Aes.Create();

        public Utility(string key)
        {
            _key = key ?? throw new ArgumentNullException(key);
        }

        public async Task EncryptFile(string path)
        {
            var byteKey = GetKey();

            using (var encryptor = _algorithm.CreateEncryptor(byteKey, _algorithm.IV))
            {
                var iv = _algorithm.IV;

                using (var encryptStream = File.Open(path + ".enc", FileMode.Create))
                {
                    await encryptStream.WriteAsync(iv, 0, iv.Length);

                    using (var csEncrypt = new CryptoStream(encryptStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var fileStream = File.OpenRead(path))
                        {
                            await fileStream.CopyToAsync(csEncrypt);
                        }
                    }
                }
            }
        }

        public async Task<Stream> DecryptFile(string path)
        {
            var byteKey = GetKey();

            var fileStream = File.OpenRead(path);
            var iv = new byte[16];
            await fileStream.ReadAsync(iv, 0, iv.Length);

            var decryptor = _algorithm.CreateDecryptor(byteKey, iv);

            return new CryptoStream(fileStream, decryptor, CryptoStreamMode.Read);
        }

        private byte[] GetKey()
        {
            var key = Encoding.UTF8.GetBytes(_key);
            var byteKey = new byte[16];
            Buffer.BlockCopy(key, 0, byteKey, 0, key.Length > 16 ? 16 : key.Length);
            return byteKey;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _algorithm.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
