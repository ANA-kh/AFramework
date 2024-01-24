# unity资源路径在不同平台
![Unity特殊文件夹在各个平台的差异总结](https://blog.csdn.net/Nbin_Newby/article/details/131458148)
## Windows

|                     | Windows                                                        |
|---------------------|----------------------------------------------------------------|
| dataPath            | 应用的appname_Data/                                               |
| persistentDataPath  | C:\Users\username\AppData\LocalLow\company name\product name   |
| streamingAssetsPath | 项目名称_data文件夹/StreamingAssets                                   |
| temporaryCachePath  | C:\Users\username\AppData\Local\Temp\company name\product name |

## Android

|                     | Android                                             |
|---------------------|-----------------------------------------------------|
| dataPath            | /data/app/package name/xxx.apk                      |
| persistentDataPath  | /storage/emulated/0/Android/data/package name/files |
| streamingAssetsPath | jar:file:///data/app/package name/xxx.apk!/assets   |
| temporaryCachePath  | /storage/emulated/0/Android/data/package name/cache |

streamingAssetsPath在Android上是被压缩打包进Apk包里的,没法直接用File类去读取的,可用UnityWebRequest去读取
https://blog.csdn.net/Glow0129/article/details/104982681

```csharp
//只能使用UnityWebRequest，无法使用file类进行操作
string filePath = Application.streamingAssetsPath + "/myFile.txt";
Uri uri = new Uri(filePath);  //这里使用uri可以简化很多操作，非常推荐，否则需要自己根据不同平台做处理
UnityWebRequest request = UnityWebRequest.Get(uri);
yield return request.SendWebRequest();
if (request.result == UnityWebRequest.Result.Success)
{
    string content = request.downloadHandler.text;
    // 对文本内容进行处理
}
else
{
    Debug.LogError("Failed to read file: " + request.error);
}

```
## iOS

|                     | iOS                                                                |
|---------------------|--------------------------------------------------------------------|
| dataPath            | /var/containers/Bundle/Application/app sandbox/xxx.app/Data/       |
| persistentDataPath  | /var/mobile/Containers/Data/Application/app sandbox/Documents      |
| streamingAssetsPath | /var/containers/Bundle/Application/app sandbox/xxx.app/Data/Raw    |
| temporaryCachePath  | /var/mobile/Containers/Data/Application/app sandbox/Library/Caches |

## Unity Editor

|                     | Unity Editor                                                   |
|---------------------|----------------------------------------------------------------|
| dataPath            | 项目文件夹/Assets                                                   |
| persistentDataPath  | C:\Users\username\AppData\LocalLow\company name\product name   |
| streamingAssetsPath | 项目文件夹/Assets/StreamingAssets                                   |
| temporaryCachePath  | C:\Users\username\AppData\Local\Temp\company name\product name |