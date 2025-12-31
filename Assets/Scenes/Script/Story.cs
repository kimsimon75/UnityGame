
using UnityEngine;

public class Story : Actor
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int[][] story = new int[14][];
    public byte level = 0;
    private ItemManager item;
    
    protected override void Start()
    {
        base.Start();
        item = GameManager.Instance.ItemManager;
        armorType = ArmorType.공성;
        story[0] = new int[2] { 0, 0 };
        story[1] = new int[2] { 100000, 9 };
        story[2] = new int[2] { 400000, 26 };
        story[3] = new int[2] { 1320000, 41 };
        story[4] = new int[2] { 3230000, 54 };
        story[5] = new int[2] { 7000000, 67 };
        story[6] = new int[2] { 18200000, 82 };
        story[7] = new int[2] { 34500000, 103 };
        story[8] = new int[2] { 72000000, 116 };
        story[9] = new int[2] { 188500000, 118 };
        story[10] = new int[2] { 300000000, 169 };
        story[11] = new int[2] { 350000000, 189 };
        story[12] = new int[2] { 430000000, 216 };
        story[13] = new int[2] { 550000000, 247 };

        currentHealth = maxHealth = story[++level][0];
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead == true)
        {
            isDead = false;

            switch (level)
            {
                case 1:
                    item.list.GetAll(3);
                    item.list.GetSoulParts(1);
                    break;
                case 2:
                    item.list.GetAll(3);
                    item.list.GetSoulParts(1);
                    item.list.GetRandomItem(ItemRank.안흔함);
                    break;
                case 3:
                    item.list.GetAll(4);
                    item.list.GetSoulParts(1);
                    StartCoroutine(item.list.GetRandomItems(ItemRank.안흔함, 2));
                    break;
                case 4:
                    item.list.GetMemoriesParts(1);
                    item.list.GetSoulParts(1);
                    item.list.GetRandomItem(ItemRank.안흔함);
                    StartCoroutine(item.list.GetRandomItems(ItemRank.특별함, 2));
                    break;
                case 5:
                    item.list.GetMemoriesParts(1);
                    item.list.GetSoulParts(1);
                    StartCoroutine(item.list.GetRandomItems(ItemRank.안흔함, 2));
                    StartCoroutine(item.list.GetRandomItems(ItemRank.특별함, 2));
                    break;
                case 6:
                    item.list.GetMemoriesParts(3);
                    item.list.GetAll(1);
                    item.list.GetRandomItem(ItemRank.희귀함);
                    break;
                case 7:
                    item.list.GetMemoriesParts(3);
                    item.list.GetAll(2);
                    StartCoroutine(item.list.GetRandomItems(ItemRank.희귀함, 2));
                    break;
                case 8:
                    item.list.GetMemoriesParts(4);
                    item.list.GetAll(1);
                    StartCoroutine(item.list.GetRandomItems(ItemRank.희귀함, 3));
                    break;
                case 9:
                    item.list.GetMemoriesParts(5);
                    item.list.GetAll(1);
                    StartCoroutine(item.list.GetRandomItems(ItemRank.안흔함, 2));
                    StartCoroutine(item.list.GetRandomItems(ItemRank.특별함, 3));
                    break;
                case 10:
                    item.list.GetMemoriesParts(4);
                    item.list.GetAll(1);
                    item.list.GetSoulParts(1);
                    break;
                case 11:
                    item.list.GetMemoriesParts(4);
                    item.list.GetAll(4);
                    item.list.GetSoulParts(1);
                    StartCoroutine(item.list.GetRandomItems(ItemRank.안흔함, 2));
                    if(GameManager.Instance.GetRound() < 30)
                    {
                        GameManager.Instance.chat.Push("파괴왕 달성");
                        item.list.GetMemoriesParts(2);
                    }
                    break;
                case 12:
                    item.list.GetMemoriesParts(3);
                    item.list.GetAll(4);
                    item.list.GetSoulParts(1);
                    StartCoroutine(item.list.GetRandomItems(ItemRank.특별함, 2));
                    break;
                case 13:
                    item.list.GetMemoriesParts(3);
                    item.list.GetAll(3);
                    item.list.GetSoulParts(1);
                    break;
            }

            currentHealth = maxHealth = story[++level][0];
        }

        Timeline();

        Armor = story[level][1] - deArmor;
    }

    public override void TakeDamageAll_physics(int damageAll, int damage, float detectRange, ArmorType damageType, float DoublePhysicsDamagePercentage)// damageAll만 사용
    {
        if (isDead) return;
        if(damageAll == 0 && damage == 0)return;

        damageAll = (int)(damageAll * GetDamage(damageType, armorType));
        damage = (int)(damage * GetDamage(damageType, armorType));            

        
        float pureDamage = damageAll;
        float doubledDamage = 0;
        if (DoublePhysicsDamagePercentage > 0)
        {
            pureDamage = damageAll*Mathf.Max(1 - DoublePhysicsDamagePercentage, 0);
            doubledDamage = damageAll * DoublePhysicsDamagePercentage;
        }
            pureDamage = pureDamage * ArmorCalculate(Armor, armorDecrease);
            damage = (int)(damage * ArmorCalculate(Armor, armorDecrease));

        currentHealth = Mathf.Max(currentHealth - damage - (DoublePhysicsDamagePercentage > 0 ? damageAll : pureDamage - doubledDamage * (1 + ArmorCalculate(Armor, armorDecrease)) ) , 0f);
        Clear();
    }
    public override void TakeDamageAll_magics(int damageAll, int damage, float radius, bool trueDamage = false)
    {
        if(isDead) return;
        if(damageAll == 0 && damage == 0)return;
        if(trueDamage)
            currentHealth = Mathf.Max(currentHealth - damage - damageAll, 0);
        else
            currentHealth = Mathf.Max(currentHealth - (damage + damageAll) * GetMagicDamage(), 0);
        Clear();
    }
    public override void TakeDamageAll_percentage(float damageAll, float damage, float radius, PercentKind percentKind, PercentageCategory percentageCategory, bool boss = false, int armorDecrease = 0, ArmorType damageType = ArmorType.패기)
    {
        if(isDead) return;
        if(damageAll == 0 && damage == 0)return;
        Debug.Log((damageAll + damage) / 100f * Percentage(percentageCategory) * DamageKind(percentKind,Armor,armorDecrease));
        currentHealth = Mathf.Max(currentHealth - (damageAll + damage) / 100f * Percentage(percentageCategory) * DamageKind(percentKind,Armor,armorDecrease,damageType,armorType), 0);
        Clear();
    }

    public override void TakeDamage_explosions(float damage, ArmorType damageType, PercentageCategory percentageCategory)
    {
        if(isDead) return;
        if( damage == 0)return;
        damage = damage * GetDamage(damageType, armorType) * ExPercentage(percentageCategory);
        TakeDamageAll_magics((int)damage, 0, 0, false);
    }

    public override void TakeStunAll(float Time, float TimeAll, float radius) { return; }
    public override void TakePoisonAll(float Time, int Armor, float radius)
    {
        deArmor = Armor;
        deArmorTime = Time;
    }

    private void Clear()
    {        
        if (currentHealth <= 0)
        {
            isDead = true;
        }
    }

    public (int story, byte level, ArmorType armorType) GetDamageInfo() { return (Armor, level, armorType); }
    
}
