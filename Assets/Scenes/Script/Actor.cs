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
    static public float GetDamage(ArmorType damageType, ArmorType armorType)
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

    public abstract void TakeDamageAll(float damageAll, float damage, float radius, ArmorType damageType);
    
}
