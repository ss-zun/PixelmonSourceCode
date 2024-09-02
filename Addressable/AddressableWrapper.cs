using UnityEngine;

public class AddressableWrapper
{
    public static bool isTest = false;

    public static string remotePath
    {
        get
        {
            string path = "";
            if (isTest)
                path = "http://newto.synology.me/DSAddressable";
            else
                path = "http://newto.synology.me/DSAddressable";

            //Debug.Log("[Debug] runtime path : " + path);
            return path;
        }
    }

    public static string appVersion
    {
        get
        {
            string version = Application.version;
            //Debug.Log("[Debug] PlayerBuildVersion : " + version);
            return version;
        }
    }

    public static string buildTarget
    {
        get
        {
            var target = Application.platform == RuntimePlatform.Android ? "Android" : Application.platform == RuntimePlatform.IPhonePlayer ? "ios" : "StandaloneWindows";
            //Debug.Log("[Debug] platform : " + target);
            return target;
        }
    }
}