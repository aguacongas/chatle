using System;
using System.IO;
using System.Threading.Tasks;

namespace ChatLe.Cryptography
{
    public interface IUtility: IDisposable
    {
        Task<Stream> DecryptFile(string path);
        Task EncryptFile(string path);
    }
}