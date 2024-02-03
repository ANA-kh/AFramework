using System;
using AFramework.ResModule.BundleResources;

namespace AFramework.ResModule.Editor.BundleModifier
{
    public class CryptographBundleModifier : IBundleModifier
    {
        private readonly Func<BundleInfo,bool> _filter;
        private readonly IEncryptor _encryptor;

        public CryptographBundleModifier(IEncryptor encryptor, Func<BundleInfo, bool> filter)
        {
            _filter = filter;
            _encryptor = encryptor;
        }
        
        
        public void Modify(BundleData bundleData)
        {
            BundleInfo bundleInfo = bundleData.BundleInfo;
            if (_filter != null && !_filter(bundleInfo))
                return;

            var data = this._encryptor.Encrypt(bundleData.Data);
            bundleData.Data = data;
            bundleInfo.FileSize = data.Length;
            bundleInfo.Encoding = this._encryptor.AlgorithmName;
        }
    }

    public interface IEncryptor
    {
        byte[] Encrypt(byte[] data);
        string AlgorithmName { get; set; }
    }
}