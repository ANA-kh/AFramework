# assetbundle
assetbundleParser: 解析(加密解密)
ResourcePathType,Priority: 资源路径类型(dataPath,streamingAssetsPath,persistentDataPath),优先级


loader类: 加载器 
ResourceResLoader
AssetBundleResLoader
AssetBundleLoader



assetLoader 加载方法 加载进度
res  资源   引用计数

Resources 资源管理器(bundle resource simulate)


问题:
如果assetbundle循环依赖,会有问题吗
异步加载后立刻同步加载