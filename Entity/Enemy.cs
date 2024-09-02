using Sirenix.OdinInspector;
using UnityEngine;

public class Enemy : SerializedMonoBehaviour
{
    public EnemyFSM fsm;
    public EnemyHealthSystem healthSystem;
    public BossHealthSystem bossHealthSystem;
    public EnemyStatHandler statHandler;
    public Collider2D enemyCollider;
    public SpriteRenderer myImg;

    private void Start()
    {
        if (fsm == null)
        {
            GetComponent<EnemyFSM>();
        }

        if (healthSystem == null )
        {
            GetComponent<EnemyHealthSystem>();
        }

        fsm.Init();
    }

    private void OnEnable()
    {
        myImg.color = Color.white;
        enemyCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PixelmonProjectile"))
        {
            ProjectileController projectile = collision.gameObject.GetComponent<ProjectileController>();
            healthSystem.TakeDamage(projectile.projectileDamage, projectile.isCri);
            projectile.ResetProjectile();
        }
    }
}