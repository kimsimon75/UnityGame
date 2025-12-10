using System;
using RaycastPro.RaySensors;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
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

    protected override void Start()
    {
        base.Start();
        walk = GetComponent<WalkForward>();
        int round = GameManager.Instance.GetRound();

        MaxHealth = DataManager.Instance.enemyStats[round][0];
        originArmor = DataManager.Instance.enemyStats[round][1];
        // 게임 시작할 때 현재 체력을 최대치로 초기화
        CurrentHealth = MaxHealth;

        player = GameManager.Instance.player;

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
    public void TakeDamage_physics(float damage, ArmorType damageType, int armorDecrease) 
    {
        if (isDead) return;
        damage = damage * GetDamage(damageType, armorType) * ArmorCalculate(Armor, armorDecrease);

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);

        Clear();
    }

    public override void TakeDamageAll_physics(int damageAll, int damage, float radius, ArmorType damageType, float DoublePhysicsDamagePercentage, int armorDecrease)
    {
        float pureDamageAll = damageAll;
        float doubledDamage = 0;
        if (DoublePhysicsDamagePercentage > 0)
        {
            pureDamageAll = damageAll*Mathf.Max(1 - DoublePhysicsDamagePercentage, 0);
            doubledDamage = damageAll * DoublePhysicsDamagePercentage;
        }
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
                    stats.TakeDamage_physics(pureDamageAll + doubledDamage * ArmorCalculate(Armor, armorDecrease) , damageType, armorDecrease);
                }
            }

            DebugDrawCircleXZ(center, radius, Color.red);
        }

        TakeDamage_physics(damage + pureDamageAll + doubledDamage * (1 + ArmorCalculate(Armor, armorDecrease)), damageType, armorDecrease);

    }      

    public void TakeDamage_magics(int damage)
    {
        if(isDead) return;
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        Clear();
    } 

    public override void TakeDamageAll_magics(int damageAll, int damage, float radius, bool trueDamage = false)
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
                    stats.TakeDamage_magics(damageAll);
                }
            }

            DebugDrawCircleXZ(center, radius, Color.red);
        }
        TakeDamage_magics(damageAll + damage);
    }    

    public void TakeDamage_explosions(float damage, ArmorType damageType, int percent = 0)/// percent 0 : 일반, 1 : 전체, 2 : 현재, 3 : 잃은, 
    {
        
    }

    public override void TakeDamageAll_percentage(float damageAll, float damage, float radius, int percent = 0) /// percent 0 : 일반, 1 : 전체, 2 : 현재, 3 : 잃은, 
    {
        
    }   
    
     public void TakeDamage_percentage(float damage, int percent = 0)
    {
        
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
    public void Clear()
    {
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
            DestroySelf();
        }
    }
    
    public void DestroySelf()
    {   
        if (isDead) return;
        --player.UnitCount;
        isDead = true;
        Destroy(bar);
        Destroy(gameObject);
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
