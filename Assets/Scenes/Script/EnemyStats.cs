using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStats : Actor
{

    [Header("체력 설정")]
    [Tooltip("최대 체력")]
    public float MaxHealth;

    [Tooltip("시작 시 현재 체력")]
    public float CurrentHealth;
    private PlayerStats player;
    private bool isDead = false;
    private int armor = 0;
    private float moveSpeed = 4.84f;

    public ArmorType armorType { private set; get; }

    [System.Obsolete]
    void Start()
    {
        MaxHealth = 100f;
        // 게임 시작할 때 현재 체력을 최대치로 초기화
        CurrentHealth = MaxHealth;

        player = FindObjectOfType<PlayerStats>();

        if (GameManager.Instance.GetRound() <= 60)
        {
            armorType = ArmorType.일반;
        }
    }

    /// <summary>
    /// 데미지를 입었을 때 호출
    /// </summary>
    public void TakeDamage(float damage, ArmorType damageType)
    {
        damage = damage * GetDamage(damageType, armorType);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);
        if (CurrentHealth <= 0 && !isDead)
        {
            --player.UnitCount;
            Destroy(gameObject);
            isDead = true;
        }
    }

    public override void TakeDamageAll(float damageAll, float damage, float radius, ArmorType damageType)
    {
        Vector3 center = transform.position;

        TakeDamage(damage + damageAll, damageType);

        // 원하는 레이어만 필터링
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        Collider[] hits = Physics.OverlapSphere(center, radius, enemyLayer);

        foreach (Collider col in hits)
        {
            EnemyStats stats = col.GetComponent<EnemyStats>();
            if (stats != null && col.transform != transform)
            {
                stats.TakeDamage(damageAll, damageType);
            }
        }

        DebugDrawCircleXZ(center, radius, Color.red);
    }

    /// <summary>
    /// 회복 아이템 등으로 체력을 회복할 때 호출
    /// </summary>
    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }



    void DebugDrawCircleXZ(Vector3 center, float radius, Color color, int segments = 36)
    {
        float delta = 2 * Mathf.PI / segments;
        Vector3 prev = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = delta * i;
            Vector3 next = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Debug.DrawLine(prev, next, color, 0.1f);
            prev = next;
        }
    }
    public (int armor, float moveSpeed, ArmorType armorType) GetDamageInfo()
    {
        return (armor, moveSpeed, armorType);
    }
}
