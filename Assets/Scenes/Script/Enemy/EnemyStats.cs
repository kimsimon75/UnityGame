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
    public float moveSpeed = 484f;
    private WalkForward walk;
    public bool boss = false;
    private bool specialBoss = false;
    private int round = 0;


    [System.Obsolete]
    void Start()
    {
        walk = GetComponent<WalkForward>();
        int round = GameManager.Instance.GetRound();

        MaxHealth = DataManager.Instance.enemyStats[round][0];
        originArmor = DataManager.Instance.enemyStats[round][1];
        // 게임 시작할 때 현재 체력을 최대치로 초기화
        CurrentHealth = MaxHealth;

        player = FindObjectOfType<PlayerStats>();

        if (round <= 60)
        {
            armorType = ArmorType.일반;
        }
    }

    void Update()
    {
        Timeline();
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
                damage = damage * 1;
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
            damage = damage * ArmorCalculate(Armor, armorDecrease);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);
        if (CurrentHealth <= 0)
        {
            if (boss)
            {
                if (specialBoss)
                {
                    int parts = 0;
                    switch (round)
                    {
                        case DataManager.삼십라운드:
                            parts = 0;
                            GameManager.Instance.ItemManager.list.GetRandomItem(ItemRank.안흔함);
                            break;
                        case DataManager.사십라운드:
                            parts = 1;
                            GameManager.Instance.ItemManager.list.GetRandomItem(ItemRank.특별함);
                            break;
                        case DataManager.오십라운드:
                            parts = 1;
                            break;
                    }
                    GameManager.Instance.ItemManager.list.GetMemoriesParts(parts);
                }
                else if (round <= 60)
                    GameManager.Instance.ItemManager.list.GetMemoriesParts(DataManager.Instance.bossReword[DataManager.Instance.bossRound++]);
            }
            --player.UnitCount;
            DestroySelf();
            isDead = true;
        }
    }

    public void DestroySelf()
    {
        Destroy(bar);
        Destroy(gameObject);
    }

    public override void TakeDamageAll(float damageAll, float damage, float radius, ArmorType damageType, bool physics,  float DoublePhysicsDamagePercentage, int armorDecrease, int percent = 0)
    {
        if (radius != 0)
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

            DebugDrawCircleXZ(center, radius, Color.red);
        }

        TakeDamage(damage + damageAll, damageType, physics, armorDecrease, percent);

    }       
    public void TakeStun(float Time)
    {
        walk.StunTime = Mathf.Max(walk.StunTime, Time);
    }


    public override void TakeStunAll(float TimeAll, float Time, float radius)
    {
        Vector3 center = transform.position;

        // 원하는 레이어만 필터링
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider[] hits = Physics.OverlapSphere(center, radius, enemyLayer);
        float realTime = Mathf.Max(Time, TimeAll * (boss ? 0.3f : 1f));

        foreach (Collider col in hits)
        {
            EnemyStats stats = col.GetComponent<EnemyStats>();

            if (stats != null && col.transform != transform)
            {
                stats.TakeStun(realTime);
            }
        }
        TakeStun(realTime);
    }


    public void TakePoison(float Time, int Armor)
    {
        deArmorTime = Mathf.Max(deArmorTime, Time);
        deArmor = Armor;
    }

    public override void TakePoisonAll(float Time, int Armor, float radius)
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
                stats.TakePoison(Time, Armor);
            }
        }
        TakePoison(Time, Armor);
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
        return (Armor, moveSpeed, armorType);
    }

    public void SetRound(int round) => this.round = round;

    public void SetBoss(bool boss) => this.boss = boss;
    public void SetSpecialBoss(bool specialBoss) => this.specialBoss = specialBoss;

    public void SetArmor(int armor) => this.originArmor = armor;
}
