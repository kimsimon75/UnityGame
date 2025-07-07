using UnityEngine;

public class Story : Actor
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float currentHealth = 0;
    public float maxHealth = 0;
    int[] story = new int[1000];
    public byte level = 0;
    bool isDead = false;
    int armor = 0;
    ArmorType armorType = ArmorType.공성;
    public ItemManager item;
    void Start()
    {
        story[0] = 100;
        story[1] = 100;
        story[2] = 100;
        story[3] = 100;
        story[4] = 100;
        story[5] = 100;
        story[6] = 100;
        story[7] = 100;
        story[8] = 100;
        story[9] = 100;
        story[10] = 100;
        story[11] = 100;
        story[12] = 100;

        for (int i = 0; i < 1000; i++)
        {
            story[i] = 1000;
        }

        currentHealth = maxHealth = story[level++];
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

            currentHealth = maxHealth = story[level++];



        }
    }

    public override void TakeDamageAll(float damageAll, float damage, float detectRange, ArmorType damageType)
    {
        damageAll = damageAll * GetDamage(damageType, armorType);
        currentHealth = Mathf.Max(currentHealth - damage - damageAll, 0f);
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
        }
    }

    public (int armor, byte level, ArmorType armorType) GetDamageInfo() { return (armor, level, armorType); }
}
