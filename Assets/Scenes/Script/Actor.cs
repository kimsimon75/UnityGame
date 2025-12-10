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

    protected virtual void Start()
    {
        magicArmor = 0.85f;
        magicalBuffer = 0f;
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

    public abstract void TakeDamageAll_percentage(float damageAll, float damage, float radius, int percent = 0);

    public abstract void TakeStunAll(float TimeAll, float Time, float radius);

    public abstract void TakePoisonAll(float Time, int Armor, float radius);

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
}
