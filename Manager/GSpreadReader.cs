using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class GSpreadExtensions
{
    public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOp)
    {
        TaskCompletionSource<UnityWebRequest.Result> tsc = new TaskCompletionSource<UnityWebRequest.Result>();
        reqOp.completed += asyncOp => tsc.TrySetResult(reqOp.webRequest.result);

        if (reqOp.isDone)
            tsc.TrySetResult(reqOp.webRequest.result);

        return tsc.Task.GetAwaiter();
    }

    public static Vector3 ToVector3(this string str)
    {
        if (str[0] == '(' && str.Last() == ')')
        {
            var pos = str.Substring(1, str.Length - 2).Split(',');
            return new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
        }
        return Vector3.zero;
    }

    public static T GetData<T>(this string rcode) where T : class, IData
    {
        return DataManager.Instance.GetData<T>(rcode);
    }

    public async static Task<T> GetAsset<T>(this string rcode) where T : class, IData
    {
        return await ResourceManager.Instance.LoadAsset<T>(rcode, eAddressableType.prefab);
    }
}

[System.Serializable]
public class SheetInfo
{
    public string className;
    public string sheetId;
    public List<Dictionary<string, string>> datas;
}

public abstract class GSpreadReader<V> : Singleton<V> where V : GSpreadReader<V>
{
    [Serializable]
    public class GameData<T> where T : class, IData
    {
        public List<T> data = new List<T>();

        public void SetData()
        {
            data = gSpread.ImportData<T>();
            gSpread.AddDataDics(data);
        }
    }

    public static GSpreadReader<V> gSpread;
    [SerializeField] private string url;
    [SerializeField] private List<SheetInfo> sheets;
    [NonSerialized] public bool isInit = false;

    protected override void Awake()
    {
        base.Awake();
        gSpread = this;
    }

    public async Task<bool> Init()
    {
        float progress = 1.0f;
        foreach (var sheet in sheets)
        {
            var url = $"{this.url}export?format=tsv&gid={sheet.sheetId}";
            var req = UnityWebRequest.Get(url);
            var op = req.SendWebRequest();
            //UILoading.Instance.SetProgress(op, $"{sheet.className} 데이터 로딩중");
            await op;

            // 네트워크 오류 또는 서버 응답 코드 확인
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching data for {sheet.className}: {req.error}");
                UILoading.Instance.SetProgress(progress++ / sheets.Count, $"{sheet.className} 데이터 로딩 실패");
                return false; // 오류 발생 시 false 반환
            }
            var res = req.downloadHandler.text;
            //Debug.Log(res);
            sheet.datas = TsvToDic(res);
            UILoading.Instance.SetProgress(progress++/sheets.Count, "데이터 불러오는 중");
        }
        ImportDatas();
        return true; // 모든 시트 로딩 성공 시 true 반환
    }

    List<Dictionary<string, string>> TsvToDic(string data)
    {
        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
        var rows = data.Split('\n');
        var keys = rows[0].Trim().Split('\t');
        for (int i = 1; i < rows.Length; i++)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var columns = rows[i].Trim().Split('\t');
            for (int j = 0; j < columns.Length; j++)
            {
                dic.Add(keys[j], columns[j]);
            }
            list.Add(dic);
        }
        return list;
    }

    protected void ImportDatas()
    {
        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            field.FieldType.GetMethod("SetData")?.Invoke(field.GetValue(this), null);
        }
        isInit = true;
    }

    public List<T> ImportData<T>()
    {
        var sheet = sheets.Find(obj => obj.className == typeof(T).Name);
        if (sheet != null)
        {
            return GetDatas<T>(sheet.datas);
        }
        return null;
    }

    public List<T> GetDatas<T>(List<Dictionary<string, string>> datas)
    {
        List<T> list = new List<T>();
        foreach (var data in datas)
        {
            list.Add(DicToClass<T>(data));
        }
        return list;
    }



    public T DicToClass<T>(Dictionary<string, string> data)
    {
        var dt = Activator.CreateInstance(typeof(T));
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var keys = new List<string>(data.Keys);
        foreach (var field in fields)
        {
            try
            {
                var idx = keys.FindIndex(obj => obj == field.Name);
                if (idx >= 0)
                {
                    if (field.FieldType == typeof(int))
                    {
                        field.SetValue(dt, int.Parse(data[keys[idx]]));
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        field.SetValue(dt, float.Parse(data[keys[idx]]));
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(dt, bool.Parse(data[keys[idx]]));
                    }
                    else if (field.FieldType == typeof(double))
                    {
                        field.SetValue(dt, double.Parse(data[keys[idx]]));
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        field.SetValue(dt, data[keys[idx]]);
                    }
                    else if (field.FieldType == typeof(List<int>))
                    {
                        var datas = data[keys[idx]].Split('|');
                        List<int> list = new List<int>();
                        foreach (var str in datas)
                        {
                            list.Add(int.Parse(str));
                        }
                        field.SetValue(dt, list);
                    }
                    else if (field.FieldType == typeof(int[]))
                    {
                        var datas = data[keys[idx]].Split('|');
                        List<int> list = new List<int>();
                        foreach (var str in datas)
                        {
                            list.Add(int.Parse(str));
                        }
                        field.SetValue(dt, list.ToArray());
                    }
                    else if (field.FieldType == typeof(List<string>))
                    {
                        field.SetValue(dt, data[keys[idx]].Split('|').ToList());
                    }
                    else if (field.FieldType == typeof(string[]))
                    {
                        field.SetValue(dt, data[keys[idx]].Split('|'));
                    }
                    else if (field.FieldType == typeof(Vector3))
                    {
                        field.SetValue(dt, data[keys[idx]].ToVector3());
                    }
                    else
                    {
                        field.SetValue(dt, Enum.Parse(field.FieldType, data[keys[idx]]));
                    }
                }
            }
            catch (Exception) { }
        }
        return (T)dt;
    }

    public abstract void AddDataDics<T>(List<T> datas) where T : class, IData;

}
