using Sirenix.OdinInspector;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ProjectileController : SerializedMonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    public BigInteger projectileDamage;
    public bool isCri;

    private Vector2 startPosition;
    private Vector2 shootDirection;
    private float flyDistance;
    private float flySpeed;
    private bool isShoot;
    private Vector3 pos;
    private void Awake()
    {
        pos = new Vector3(-1000, 0, 0);
        gameObject.transform.position = pos;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        if(!isShoot) return;
        rb.velocity = shootDirection.normalized * flySpeed;

        // 발사체가 일정 거리 이상 날아가면 비활성화
        if (Vector2.Distance(startPosition, rb.position) >= flyDistance)
        {
            ResetProjectile();
        }
    }

    //Invoke쪽에서 전달해야 할 변수들.
    public void GetAttackSign(Vector3 startPos, Vector2 direction, BigInteger damage, bool isCritical ,float bulletRange, float speed)
    {
        startPosition = startPos; //날아가기 시작하는 지점.
        shootDirection = direction; //날아갈 방향.
        projectileDamage = damage; //전달할 데이터.
        flyDistance = bulletRange; //날아갈 거리.
        flySpeed = speed; //날아갈 속도.
        isCri = isCritical;
        rb.position = startPosition;
        isShoot = true; 
    }

    // 발사체의 위치와 속도를 초기화하는 메서드
    public void ResetProjectile()
    {
        rb.velocity = Vector2.zero;
        isShoot = false;
        transform.position = pos;
        gameObject.SetActive(false);
    }
}
