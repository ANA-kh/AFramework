using AFramework.ResModule.BundleResources;

namespace AFramework.ResModule.Editor.BundleModifier
{
    public class BundleData
    {
        private byte[] data;
        private BundleInfo bundleInfo;
        public BundleData(BundleInfo bundleInfo, byte[] data)
        {
            this.bundleInfo = bundleInfo;
            this.data = data;
        }
        public byte[] Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        public BundleInfo BundleInfo { get { return this.bundleInfo; } }
    }
}