using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public string tag;
    public string rcode;
    public SerializedMonoBehaviour prefab;
    public int size;
}

public class PoolManager : Singleton<PoolManager>
{
    public List<Pool> Pools;
    public Dictionary<string, Queue<SerializedMonoBehaviour>> PoolDictionary;
    private Transform objectPoolParent;

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();

        PoolDictionary = new Dictionary<string, Queue<SerializedMonoBehaviour>>();
        objectPoolParent = new GameObject("ObjectPool").transform;
    }

    private void Start()
    {
        foreach (var pool in Pools)
        {
            CreatePool(pool);
        }
    }

    public T SpawnFromPool<T>(string rcode) where T : SerializedMonoBehaviour
    {
        if (!PoolDictionary.ContainsKey(rcode)) return default;

        SerializedMonoBehaviour obj = PoolDictionary[rcode].Dequeue();
        PoolDictionary[rcode].Enqueue(obj);
        obj.gameObject.SetActive(true);
        return (T)obj;
    }

    private void CreatePool(Pool pool)
    {
        if (PoolDictionary.ContainsKey(pool.rcode)) return;

        Queue<SerializedMonoBehaviour> objectPool = new Queue<SerializedMonoBehaviour>();
        Transform rcodeParent = new GameObject(pool.rcode).transform;
        if (pool.tag == "DamageTxt")
        {
            rcodeParent.SetParent(UIManager.Instance.canvas.transform);
            rcodeParent.gameObject.transform.SetAsFirstSibling();
        }
        else
            rcodeParent.SetParent(objectPoolParent);


        for (int i = 0; i < pool.size; i++)
        {
            SerializedMonoBehaviour obj = Instantiate(pool.prefab, rcodeParent);
            obj.gameObject.SetActive(false);
            objectPool.Enqueue(obj);
        }

        PoolDictionary.Add(pool.rcode, objectPool);
    }
}
