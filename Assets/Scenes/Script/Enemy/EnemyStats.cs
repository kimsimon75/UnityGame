using System;
using RaycastPro.RaySensors;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EnemyStats : Actor
{
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

        maxHealth = DataManager.Instance.enemyStats[round][0];
        originArmor = DataManager.Instance.enemyStats[round][1];
        // 게임 시작할 때 현재 체력을 최대치로 초기화
        currentHealth = maxHealth;

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
    private void TakeDamage_physics(int damage, ArmorType damageType, int armorDecrease) 
    {
        if (isDead) return;
        damage = (int)(damage * GetDamage(damageType, armorType) * ArmorCalculate(Armor, armorDecrease));

        currentHealth = (int)Mathf.Max(currentHealth - damage, 0f);

        Clear();
    }

    public override void TakeDamageAll_physics(int damageAll, int damage, float radius, ArmorType damageType, float DoublePhysicsDamagePercentage, int armorDecrease)
    {
        int pureDamageAll = damageAll;
        int doubledDamage = 0;
        if (DoublePhysicsDamagePercentage > 0)
        {
            pureDamageAll = (int)(damageAll * Mathf.Max(1 - DoublePhysicsDamagePercentage, 0));
            doubledDamage = (int)(damageAll * DoublePhysicsDamagePercentage);
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
                    stats.TakeDamage_physics(pureDamageAll + (int)(doubledDamage * ArmorCalculate(Armor, armorDecrease)) , damageType, armorDecrease);
                }
            }

            DebugDrawCircleXZ(center, radius, Color.red);
        }

        TakeDamage_physics(damage + pureDamageAll + (int)(doubledDamage * Mathf.Pow(ArmorCalculate(Armor, armorDecrease), 2)), damageType, armorDecrease);

    }      

    private void TakeDamage_magics(int damage, bool trueDamage = false)
    {
        if(isDead) return;
        if(trueDamage)
            currentHealth = Mathf.Max(currentHealth - damage, 0);
        else
            currentHealth = Mathf.Max(currentHealth - damage * GetMagicDamage(), 0);
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
                EnemyStats stats = col.GetComponentInParent<EnemyStats>();
                if (stats != null && stats != this)
                {
                    stats.TakeDamage_magics(damageAll, trueDamage);
                }
            }

            DebugDrawCircleXZ(center, radius, Color.red);
        }
        TakeDamage_magics(damageAll + damage, trueDamage);
    }    

    public override void TakeDamage_explosions(float damage, ArmorType damageType, int percent = 0)/// percent 0 : 일반, 1 : 전체, 2 : 현재, 3 : 잃은, 
    {
        if(isDead) return;
        damage = damage * GetDamage(damageType, armorType) * (boss ? ExPercentage(percent) : Percentage(percent));
        
        currentHealth = (int)Mathf.Max(currentHealth - damage, 0f);
        Clear();
    }

    public override void TakeDamageAll_percentage(float damageAll, float damage, float radius, int damageKind, int percent, bool boss = false, int armorDecrease = 0, ArmorType damageType = ArmorType.패기) /// percent 0 : 일반, 1 : 전체, 2 : 현재, 3 : 잃은, damageKind 0: 물리, 1: 마법, 2: 고정
    {
        if (radius != 0)
        {
            Vector3 center = transform.position;


            // 원하는 레이어만 필터링
            LayerMask enemyLayer = LayerMask.GetMask("Enemy");

            Collider[] hits = Physics.OverlapSphere(center, radius, enemyLayer);

            foreach (Collider col in hits)
            {
                EnemyStats stats = col.GetComponentInParent<EnemyStats>();
                if (stats != null)
                {
                    stats.TakeDamage_percentage(damageAll, damageKind, percent, stats.boss, armorDecrease, damageType);
                }
            }

            DebugDrawCircleXZ(center, radius, Color.red);
        }
        else
            TakeDamage_percentage(damageAll + damage, damageKind, percent, boss, armorDecrease, damageType);
    }   
    
     private void TakeDamage_percentage(float damage, int damageKind, int percent, bool boss = false, int armorDecrease = 0, ArmorType damageType = ArmorType.패기) // damageKind 0: 물리, 1: 마법, 2: 고정, 3: 폭발(폭발은 따로 설정)
    {
        if(isDead) return;

        if(this.boss == boss)
        {
            if(damageKind == 0) damage = damage * ArmorCalculate(Armor, armorDecrease);
        
            damage = damage / 100  * Percentage(percent) * DamageKind(damageKind, Armor, armorDecrease, damageType, armorType);
        }    
        
        currentHealth = (int)Mathf.Max(currentHealth - damage, 0);
        Clear();

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
        currentHealth = (int)Mathf.Min(currentHealth + amount, maxHealth);
    }
    public void Clear()
    {
        if (currentHealth <= 0)
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
