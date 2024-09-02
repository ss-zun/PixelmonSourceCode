using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDetectState : IdleState
{
    private new PlayerFSM fsm;
    public float currentDetectionRadius;
    private WaitForSeconds detectionInterval = new WaitForSeconds(0.5f);

    public PlayerDetectState(PlayerFSM fsm)
        : base(fsm)
    {
        this.fsm = fsm;
    }

    public override void Enter()
    {
        base.Enter();
        fsm.target = null;
        Player.Instance.SetPixelmonsTarget(null);
        Player.Instance.ChangePixelmonsState(PixelmonState.Idle);
    }

    public override void Execute()
    {
        base.Execute();
        fsm.StartCoroutine(DetectClosestTargetCoroutine());
    }

    private IEnumerator DetectClosestTargetCoroutine()
    {
        currentDetectionRadius = fsm.initialDetectionRadius;

        while (fsm.target == null && currentDetectionRadius <= fsm.maxDetectionRadius)
        {
            fsm.target = FindClosestTarget(fsm.EnemyTag, currentDetectionRadius);
            if (fsm.target != null)
            {
                Player.Instance.SetPixelmonsTarget(fsm.target);
                fsm.ChangeState(fsm.MoveState);
                yield break;
            }
            currentDetectionRadius += fsm.radiusIncrement;
            yield return detectionInterval;
        }

        // 최대 탐지 반경까지 찾지 못한 경우, currentDetectionRadius를 초기화
        currentDetectionRadius = fsm.initialDetectionRadius;
    }

    // 범위 탐색
    private GameObject FindClosestTarget(string enemyTag, float detectionRadius)
    {
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        Vector2 playerPosition = fsm.transform.position;

        // 탐지 반경 내 모든 오브젝트 찾기
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(playerPosition, detectionRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(enemyTag))
            {
                float distanceToEnemy = Vector2.Distance(playerPosition, hitCollider.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hitCollider.gameObject;
                }
            }
        }

        return closestEnemy;
    }
}