using UnityEngine;

   public enum ArmorType
    {
        관통,
        공성,
        패기,
        일반,
    }

public abstract class Actor : MonoBehaviour
{
    public ArmorType armorType = ArmorType.일반;
    public bool isDead = false;

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
        else return 1.05f;
    }

    protected float ArmorCalculate(int Armor, int armorDecrease)
    {
        if (Armor >= armorDecrease)
            return 100f / (100f + 2f * (Armor - armorDecrease));
        else
            return 2 - Mathf.Pow(0.94f, armorDecrease - Armor);
    }

    public abstract void TakeDamageAll(float damageAll, float damage, float radius, ArmorType damageType, bool physics, int armorDecrease, int percent = 0);/// percent 0 : 일반, 1 : 전체, 2 : 현재, 3 : 잃은,

    public abstract void TakeStunAll(float TimeAll, float Time, float radius);
}
