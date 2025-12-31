using System;
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
    [NonSerialized] public ArmorType armorType = ArmorType.일반;
    [NonSerialized] public bool isDead = false;

    [NonSerialized] public int originArmor = 0;
    [NonSerialized] public int deArmor = 0;
    [NonSerialized] public float deArmorTime = 0;

    protected int Armor = 0;
    [NonSerialized] public GameObject bar;

    [NonSerialized] public float magicArmor;
    [NonSerialized] public float magicalBuffer;

    [NonSerialized] public float maxHealth;
    [NonSerialized] public float currentHealth;
    protected int armorDecrease = 0;

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

    public abstract void TakeDamageAll_physics(int damageAll, int damage, float radius, ArmorType damageType, float DoublePhysicsDamagePercentage);/// percent 0 : 일반, 1 : 전체, 2 : 현재, 3 : 잃은,

    public abstract void TakeDamageAll_magics(int damageAll, int damage, float radius, bool trueDamage = false);

    public abstract void TakeDamageAll_percentage(float damageAll, float damage, float radius, PercentKind damageKind, PercentageCategory percentCategory, bool boss = false, int armorDecrease = 0, ArmorType damageType = ArmorType.패기);

    public abstract void TakeStunAll(float TimeAll, float Time, float radius);

    public abstract void TakePoisonAll(float Time, int Armor, float radius);

    public abstract void TakeDamage_explosions(float damage, ArmorType damageType, PercentageCategory percentCategory = 0);

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
        Armor = originArmor - deArmor - armorDecrease;
    }

    protected float Percentage(PercentageCategory percent)
    {
        switch(percent)
        {
            case PercentageCategory.max:
                return maxHealth;
            case PercentageCategory.current:
                return currentHealth;
            case PercentageCategory.loss:
                return maxHealth - currentHealth;
            default:
                return 1;

        }
    }

    protected float ExPercentage(PercentageCategory percent)
    {
        switch(percent)
        {
            case PercentageCategory.max:
                return DataManager.exPercent[1];
            case PercentageCategory.current:
                return DataManager.exPercent[2];
            case PercentageCategory.loss:
                return DataManager.exPercent[3];
            default:
                return DataManager.exPercent[0];

        }
    }

    protected float DamageKind(PercentKind damageKind, int Armor = 0, int armorDecrease = 0, ArmorType damageType = ArmorType.고정, ArmorType armorType = ArmorType.고정)
    {
        switch(damageKind)
        {
            case PercentKind.physics:
                return ArmorCalculate(Armor, armorDecrease);
            case PercentKind.magics:
                return GetMagicDamage();
            case PercentKind.trueDamage:
                return 1;
            case PercentKind.explosions:
                return GetDamage(damageType, armorType);
            default:
                Debug.Assert(true);
                return 1;
        }
    }
}
