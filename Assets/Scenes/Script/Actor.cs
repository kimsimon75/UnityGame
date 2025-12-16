using Unity.VisualScripting;
using UnityEngine;

public enum ArmorType
{
    관통,
    공성,
    패기,
    일반,
    영웅,
    보스,
    마법,
    고정,
    }

public abstract class Actor : MonoBehaviour
{
    public ArmorType armorType = ArmorType.일반;
    public bool isDead = false;

    public int originArmor = 0;
    public int deArmor = 0;
    public float deArmorTime = 0;

    protected int Armor = 0;
    public GameObject bar;

    public float magicArmor;
    public float magicalBuffer;

    public float maxHealth;
    public float currentHealth;

    protected virtual void Start()
    {
        magicArmor = 0.85f;
        magicalBuffer = 1f;
    }

    protected float GetMagicDamage()
    {
        return magicArmor * magicalBuffer;
    }

    protected float GetDamage(ArmorType damageType, ArmorType armorType)
    {
        if (damageType == ArmorType.관통)
        {
            switch (armorType)
            {
                case ArmorType.관통:
                    return 1.25f;
                case ArmorType.공성:
                    return 1f;
                case ArmorType.일반:
                    return 0.75f;
                default:
                    return 1f;

            }
        }
        else if (damageType == ArmorType.공성)
        {
            switch (armorType)
            {
                case ArmorType.관통:
                    return 0.75f;
                case ArmorType.공성:
                    return 1.25f;
                case ArmorType.일반:
                    return 1f;
                default:
                    return 1f;
            }
        }
        else if (damageType == ArmorType.일반)
        {
            switch (armorType)
            {
                case ArmorType.관통:
                    return 1f;
                case ArmorType.공성:
                    return 0.75f;
                case ArmorType.일반:
                    return 1.25f;
                default:
                    return 1f;
            }
        }
        else if (damageType == ArmorType.고정) return 1f;
        else return 1.05f;
    }

    protected float ArmorCalculate(int Armor, int armorDecrease)
    {
        if (Armor >= armorDecrease)
            return 100f / (100f + 2f * (Armor - armorDecrease));
        else
            return 2 - Mathf.Pow(0.94f, armorDecrease - Armor);
    }

    public abstract void TakeDamageAll_physics(int damageAll, int damage, float radius, ArmorType damageType, float DoublePhysicsDamagePercentage, int armorDecrease);/// percent 0 : 일반, 1 : 전체, 2 : 현재, 3 : 잃은,

    public abstract void TakeDamageAll_magics(int damageAll, int damage, float radius, bool trueDamage = false);

    public abstract void TakeDamageAll_percentage(float damageAll, float damage, float radius, int damageKind, int percentCategory, bool boss = false, int armorDecrease = 0, ArmorType damageType = ArmorType.패기);

    public abstract void TakeStunAll(float TimeAll, float Time, float radius);

    public abstract void TakePoisonAll(float Time, int Armor, float radius);

    public abstract void TakeDamage_explosions(float damage, ArmorType damageType, int percentCategory = 0);

    protected void Timeline()
    {
        if (deArmorTime > 0)
        {
            deArmorTime -= Time.deltaTime;
        }
        else
        {
            deArmor = 0;
        }
        Armor = originArmor - deArmor;
    }

    protected float Percentage(int percent)
    {
        switch(percent)
        {
            case 1:
                return maxHealth;
            case 2:
                return currentHealth;
            case 3:
                return maxHealth - currentHealth;
            default:
                return 1;

        }
    }

    protected float ExPercentage(int percent)
    {
        switch(percent)
        {
            case 1:
                return DataManager.exPercent[1];
            case 2:
                return DataManager.exPercent[2];
            case 3:
                return DataManager.exPercent[3];
            default:
                return DataManager.exPercent[0];

        }
    }

    protected float DamageKind(int damageKind, int Armor = 0, int armorDecrease = 0, ArmorType damageType = ArmorType.고정, ArmorType armorType = ArmorType.고정)
    {
        switch(damageKind)
        {
            case 0:
                return ArmorCalculate(Armor, armorDecrease);
            case 1:
                return GetMagicDamage();
            case 2:
                return 1;
            case 3:
                return GetDamage(damageType, armorType);
            default:
                Debug.Assert(true);
                return 1;
        }
    }
}
