using UnityEngine;

public class Story : Actor
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float currentHealth = 0;
    public float maxHealth = 0;
    int[][] story = new int[14][];
    public byte level = 0;
    bool isDead = false;
    ArmorType armorType = ArmorType.공성;
    public ItemManager item;
    void Start()
    {
        story[0] = new int[2]{ 0, 0};
        story[1] = new int[2]{ 100000, 9};
        story[2] = new int[2]{ 400000, 26};
        story[3] = new int[2]{ 1320000, 41};
        story[4] = new int[2]{ 3230000,54};
        story[5] = new int[2]{ 7000000, 67};
        story[6] = new int[2]{ 18200000, 82};
        story[7] = new int[2]{ 34500000, 103};
        story[8] = new int[2]{ 72000000, 116};
        story[9] = new int[2]{ 188500000, 118};
        story[10] = new int[2]{ 300000000, 169};
        story[11] = new int[2]{ 350000000, 189};
        story[12] = new int[2]{ 430000000, 216};
        story[13] = new int[2]{ 550000000, 247};

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
                    item.list.FindItem("만물석").count += 3;
                    item.Clear(item.GetEditItem());
                    break;
            }

            currentHealth = maxHealth = story[++level][0];
        }
    }

    public override void TakeDamageAll(float damageAll, float damage, float detectRange, ArmorType damageType, bool physics, int armorDecrease)
    {
        if (isDead) return;
        damageAll = damageAll * GetDamage(damageType, armorType);
        if (physics)
            damageAll = damageAll * ArmorCalculate(story[level][1], armorDecrease);
        currentHealth = Mathf.Max(currentHealth - damage - damageAll, 0f);
        if (currentHealth <= 0)
        {
            isDead = true;
        }
    }

    public (int[][] story, byte level, ArmorType armorType) GetDamageInfo() { return (story, level, armorType); }
}
