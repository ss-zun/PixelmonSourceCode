using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public class Spawner : MonoBehaviour
{
    private StageManager stageManager;
    private Camera cam;

    private List<Enemy> isActivatedEnemy = new List<Enemy>();
    [SerializeField] private GameObject[] dungeonBoss;

    public GameObject points;
    [SerializeField] private Collider2D spawnArea;
    [SerializeField] private Collider2D cleanArea;

    private void Awake()
    {
        stageManager = StageManager.Instance;   
        cam = Camera.main;
    }

    public async void SpawnMonsterTroop(string[] rcodes, int totalCount)
    {
        if (stageManager.curSpawnCount >= totalCount) return;

        //몬스터 종류
        int randEnemyType = Random.Range(0, rcodes.Length);
        EnemyData curEnemy = DataManager.Instance.GetData<EnemyData>($"{rcodes[randEnemyType]}");

        //몬스터그룹 마리 수
        int maxSpawnNum = Mathf.Min(totalCount - stageManager.curSpawnCount + 1, 6);
        int randSpawnCount = Random.Range(1, maxSpawnNum);
        
        //몬스터 위치
        Vector2 RandomPos;
        do
        {
            RandomPos = GetRandomPos(spawnArea);
        }
        while (cleanArea.bounds.Contains(RandomPos));

        points.transform.position = RandomPos;
        Transform[] spawnPoints = points.GetComponentsInChildren<Transform>();

        //실제 소환
        for (int i = 0; i < randSpawnCount; i++)
        {
            if (i >= spawnPoints.Length) break;

            Enemy enemy = PoolManager.Instance.SpawnFromPool<Enemy>("Enemy");
            while (enemy == null)
            {
                enemy = PoolManager.Instance.SpawnFromPool<Enemy>("Enemy");
                await Task.Yield(); 
            }

            enemy.transform.position = spawnPoints[i].position;
            enemy.fsm.anim.runtimeAnimatorController = await ResourceManager.Instance.LoadAsset<RuntimeAnimatorController>(curEnemy.rcode, eAddressableType.animator);
            enemy.statHandler.data = curEnemy;
            enemy.statHandler.UpdateEnemyStats();
            isActivatedEnemy.Add(enemy);

            if (enemy.statHandler.data.isBoss)
            {
                enemy.bossHealthSystem.InvokeBossHp();
            }
            stageManager.curSpawnCount++;
        }
    }

    private Vector2 GetRandomPos(Collider2D collider)
    {
        Bounds bounds = collider.bounds;
        return new Vector2(Random.Range(bounds.min.x, bounds.max.x), (Random.Range(bounds.min.y, bounds.max.y)));
    }

    public void RemoveActiveMonster(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        isActivatedEnemy.Remove(enemy);
    }

    public void ResetSpawnedMonster()
    {
        for (int i = 0; i < isActivatedEnemy.Count; i++)
        {
            isActivatedEnemy[i].gameObject.SetActive(false);
        }
        isActivatedEnemy.Clear();
    }

    private IEnumerator FadeMonster(Enemy enemy)
    {
        enemy.fsm.ChangeState(enemy.fsm.AttackState);
        enemy.myImg.DOFade(0, 1f);
        yield return new WaitForSeconds(1f);
        enemy.gameObject.SetActive(false);
    }

    public DgMonster GetDgMonster(int index)
    {
        GameObject boss = Instantiate(dungeonBoss[index]);
        return boss.GetComponent<DgMonster>();
    }
}
