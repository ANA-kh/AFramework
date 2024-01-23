using System;
using AFramework.ResModule.Editor.BundleModifier;

namespace AFramework.ResModule.Editor.Security
{
    public class AESCryptograph : IEncryptor
    {
        public AESCryptograph(string key, string iv)
        {
            
        }

        public byte[] Encrypt(byte[] data)
        {
            throw new NotImplementedException();
        }

        public string AlgorithmName { get; set; }
    }
}