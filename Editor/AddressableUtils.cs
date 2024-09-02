using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System;
using System.Linq;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Build;
using System.Threading.Tasks;
using UnityEditor.AddressableAssets;

internal class AddressableUtils : Editor
{
    const string assetBundlePath = "Assets/AddressableDatas";

    [MenuItem("Tools/Addressable/Addressable Build")]
    internal static void BuildAddressable()
    {
        Mapping();
        AddressableAssetSettingsDefaultObject.Settings.OverridePlayerVersion = Application.version;
        AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
        var uploadList = result.FileRegistry.GetFilePaths().ToList();
        uploadList.RemoveAll(obj => !obj.Contains("ServerData"));
        var list = "";
        uploadList.ForEach(obj => list += obj + "\n");
        var path = Application.dataPath + "/lastBuildData.txt";
        File.WriteAllText(path, list);
    }

    [MenuItem("Tools/Addressable/Addressable Build And Upload")]
    internal static void BuildAndUploadAddressable()
    {
        BuildAddressable();
        UploadLastBuild();
    }

    [MenuItem("Tools/Addressable/Addressable Upload")]
    public async static void UploadLastBuild()
    {
        var path = Application.dataPath + "/lastBuildData.txt";
        var uploadList = File.ReadAllText(path).Split('\n').ToList();
        var type = GetBuildTargetToString();
        await DeleteFileinFolder(type);
        await CreateFolder(type);

        for (int i = 0; i < uploadList.Count; i++)
        {
            if (uploadList[i].Length > 0)
                await Upload(type, uploadList[i].Replace('\\', '/'));
        }
    }

    [InitializeOnEnterPlayMode]
    [MenuItem("Tools/Addressable/Mapping")]
    internal static void Mapping()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        foreach (var group in settings.groups)
        {
            foreach (var entry in group.entries)
            {
                if (!entry.AssetPath.Contains("Assets") || entry.AssetPath.Contains("addressableMap")) continue;
                var type = (eAddressableType)Enum.Parse(typeof(eAddressableType), group.Name);
                var dir = Application.dataPath + entry.AssetPath.Replace("Assets", "");
                var mapData = SetMapping(dir, type);
                var newPath = entry.AssetPath + "/addressableMap.json";
                var dt = JsonUtility.ToJson(mapData);
                File.WriteAllText(newPath, dt);
                AssetDatabase.ImportAsset(newPath);
            }

        }
    }

    static AddressableMapData SetMapping(string dir, eAddressableType type)
    {
        var files = Directory.GetFiles(dir).ToList();
        files.RemoveAll(obj => obj.Contains(".meta"));
        AddressableMapData mapData = new AddressableMapData();
        var dirs = Directory.GetDirectories(dir).ToList();
        foreach (var d in dirs)
        {
            if (d.Contains("Atlas")) continue;
            var res = SetMapping(d, type);
            mapData.AddRange(res.list);
        }
        foreach (var file in files)
        {
            var path = file.Replace(Application.dataPath, "Assets").Replace("\\", "/");
            var data = new AddressableMap();
            data.addressableType = type;
            data.assetType = path.Contains(".png") ? eAssetType.sprite : path.Contains(".json") ? eAssetType.jsondata : eAssetType.prefab;
            data.key = path.Split('/').Last().Split('.')[0].ToLower().Split('.').First();
            data.path = path;
            mapData.Add(data);
        }
        return mapData;
    }

    
    public static string GetBuildTargetToString()
    {
#if UNITY_EDITOR
        return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ? "Android" : EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS ? "IOS" : "StandaloneWindows";
#else
        return Application.platform == RuntimePlatform.Android ? "Android" : Application.platform == RuntimePlatform.IPhonePlayer ? "IOS" : "StandaloneWindows";
#endif
    }

    #region Use NAS Server
    public async static Task DeleteFileinFolder(string type)
    {
        var ftpUrl = new Uri("ftp://localhost/web/Addressable/");
        var request = WebRequest.Create(ftpUrl + type + "/" + Application.version) as FtpWebRequest;
        request.Credentials = new NetworkCredential("wocjf84", "afsd2977");
        request.Method = WebRequestMethods.Ftp.ListDirectory;
        request.KeepAlive = false;
        request.UseBinary = true;

        try
        {
            // 이제 디렉토리를 만든다고 FTP로 요청을 합니다.
            FtpWebResponse res = (FtpWebResponse)await request.GetResponseAsync();
            var response = res.GetResponseStream();
            var reader = new StreamReader(response);
            var list = reader.ReadToEnd().Split('\n');
            reader.Close();
            response.Close();
            res.Dispose();
            for (int i = 0; i < list.Length; i++)
            {
                var request2 = WebRequest.Create(ftpUrl + type + "/" + list[i]) as FtpWebRequest;
                request2.Credentials = new NetworkCredential("wocjf84", "afsd2977");
                request2.Method = WebRequestMethods.Ftp.DeleteFile;
                request2.KeepAlive = false;
                request2.UseBinary = true;
                FtpWebResponse res2 = (FtpWebResponse)await request2.GetResponseAsync();
                res2.Dispose();
            }
        }
        catch (WebException ex)
        {
            // 예외처리.
            FtpWebResponse response = (FtpWebResponse)ex.Response;

            switch (response.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                    {
                        //Debug.Log("DeleteFolder ] Probably the folder already exist : ");
                    }
                    break;
            }
            response.Dispose();
        }
    }

    public async static Task DeleteFolder(string type)
    {
        await DeleteFileinFolder(type);
        var ftpUrl = new Uri("ftp://localhost/web/Addressable/");
        var request = WebRequest.Create(ftpUrl + type + "/" + Application.version) as FtpWebRequest;

        request.Credentials = new NetworkCredential("wocjf84", "afsd2977");
        request.Method = WebRequestMethods.Ftp.RemoveDirectory;
        request.KeepAlive = false;
        request.UseBinary = true;

        try
        {
            // 이제 디렉토리를 만든다고 FTP로 요청을 합니다.
            FtpWebResponse res = (FtpWebResponse)await request.GetResponseAsync();
            res.Dispose();
        }
        catch (WebException ex)
        {
            // 예외처리.
            FtpWebResponse response = (FtpWebResponse)ex.Response;

            switch (response.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                    {
                        //Debug.Log("DeleteFolder ] Probably the folder already exist : ");
                    }
                    break;
            }
            response.Dispose();
        }
    }

    public async static Task CreateFolder(string type)
    {
        var ftpUrl = new Uri("ftp://localhost/web/Addressable/");
        var request = WebRequest.Create(ftpUrl + type + "/" + Application.version) as FtpWebRequest;

        request.Credentials = new NetworkCredential("wocjf84", "afsd2977");
        request.Method = WebRequestMethods.Ftp.MakeDirectory;
        request.KeepAlive = false;
        request.UseBinary = true;

        try
        {
            // 이제 디렉토리를 만든다고 FTP로 요청을 합니다.
            FtpWebResponse res = (FtpWebResponse)await request.GetResponseAsync();
            res.Dispose();
        }
        catch (WebException ex)
        {
            // 예외처리.
            FtpWebResponse response = (FtpWebResponse)ex.Response;

            switch (response.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                    {
                        //Debug.Log("CreateFolders ] Probably the folder already exist : ");
                    }
                    break;
            }
            response.Dispose();
        }
    }

    public async static Task Upload(string type, string path)
    {
        var target = File.ReadAllBytes(path);
        var ftpUrl = new Uri("ftp://localhost/web/Addressable/");
        var request = WebRequest.Create(ftpUrl + type + "/" + Application.version + "/" + path.Split('/').Last()) as FtpWebRequest;

        request.Credentials = new NetworkCredential("wocjf84", "afsd2977");
        request.Method = WebRequestMethods.Ftp.UploadFile;
        request.KeepAlive = false;
        request.UseBinary = true;
        request.ContentLength = target.Length;

        using (var ftpStream = request.GetRequestStream())
        {
            await ftpStream.WriteAsync(target, 0, target.Length);
            ftpStream.Close();
        }
        try
        {
            // 이제 디렉토리를 만든다고 FTP로 요청을 합니다.
            FtpWebResponse res = (FtpWebResponse)await request.GetResponseAsync();
            res.Dispose();
        }
        catch (WebException ex)
        {
            // 예외처리.
            FtpWebResponse response = (FtpWebResponse)ex.Response;

            switch (response.StatusCode)
            {
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                    {
                        //Debug.Log("CreateFolders ] Probably the folder already exist : ");
                    }
                    break;
            }
            response.Dispose();
        }
    }
    #endregion
}