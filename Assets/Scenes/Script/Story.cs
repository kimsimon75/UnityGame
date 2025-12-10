using UnityEngine;

public class Story : Actor
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float currentHealth = 0;
    public float maxHealth = 0;
    int[][] story = new int[14][];
    public byte level = 0;
    private ItemManager item;

    private int degree;
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

        degree = DataManager.degree;
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
                    break;
            }

            currentHealth = maxHealth = story[++level][0];
        }

        Timeline();

        Armor = story[level][1] - deArmor;
    }

    public override void TakeDamageAll_physics(int damageAll, int damage, float detectRange, ArmorType damageType, float DoublePhysicsDamagePercentage, int armorDecrease)// damageAll만 사용
    {
        if (isDead) return;

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
        if (currentHealth <= 0)
        {
            isDead = true;
        }
    }

    public override void TakeDamageAll_magics(int damageAll, int damage, float radius, bool trueDamage = false)
    {
        
    }
    public override void TakeDamageAll_percentage(float damageAll, float damage, float radius, int percent = 0)
    {
        
    }


    public override void TakeStunAll(float Time, float TimeAll, float radius) { return; }
    public override void TakePoisonAll(float Time, int Armor, float radius)
    {
        deArmor = Armor;
        deArmorTime = Time;
    }

    public (int story, byte level, ArmorType armorType) GetDamageInfo() { return (Armor, level, armorType); }
    
}
