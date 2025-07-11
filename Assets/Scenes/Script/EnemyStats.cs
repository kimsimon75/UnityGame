using System;
using RaycastPro.RaySensors;
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



    [System.Obsolete]
    void Start()
    {

        int round = GameManager.Instance.GetRound();

        SetStats(round);

        MaxHealth = DataManager.Instance.enemyStats[round][0];
        armor = DataManager.Instance.enemyStats[round][1];
        // 게임 시작할 때 현재 체력을 최대치로 초기화
        CurrentHealth = MaxHealth;

        player = FindObjectOfType<PlayerStats>();

        if (round <= 60)
        {
            armorType = ArmorType.일반;
        }
    }

    /// <summary>
    /// 데미지를 입었을 때 호출
    /// </summary>
    public void TakeDamage(float damage, ArmorType damageType, bool physics, int armorDecrease, int percent = 0) /// percent 0 : 일반, 1 : 전체, 2 : 현재, 3 : 잃은, 
    {
        if (isDead) return;
        damage = damage * GetDamage(damageType, armorType);

        switch (percent)
        {
            case 0:
                break;
            case 1:
                damage = damage / 100 * MaxHealth;
                break;
            case 2:
                damage = damage / 100 * CurrentHealth;
                break;
            case 3:
                damage = damage / 100 * (MaxHealth - CurrentHealth);
                break;
            default:
                break;
        }
        if (physics)
            damage = damage * ArmorCalculate(armor, armorDecrease);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);
        if (CurrentHealth <= 0)
        {
            --player.UnitCount;
            Destroy(gameObject);
            isDead = true;
        }
    }

    public override void TakeDamageAll(float damageAll, float damage, float radius, ArmorType damageType, bool physics, int armorDecrease, int percent = 0)
    {
        Vector3 center = transform.position;


        // 원하는 레이어만 필터링
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        Collider[] hits = Physics.OverlapSphere(center, radius, enemyLayer);

        foreach (Collider col in hits)
        {
            EnemyStats stats = col.GetComponent<EnemyStats>();
            if (stats != null && col.transform != transform)
            {
                stats.TakeDamage(damageAll, damageType, physics, armorDecrease);
            }
        }
        TakeDamage(damage + damageAll, damageType, physics, armorDecrease, percent);

        DebugDrawCircleXZ(center, radius, Color.red);
    }

    public override void TakeStun(float Time, float TimeAll, float radius, bool boss)
    {
        throw new NotImplementedException();
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

    private void SetStats(int round)
    {
        switch (round)
        {
            case 1:
                MaxHealth = 100f;
                armor = 0;
                break;
        }
    }
}
