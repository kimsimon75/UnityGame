using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [NonSerialized] public int CurrentHealth;
    [NonSerialized] public int MaxHealth;
    [NonSerialized] public int CurrentMana;
    [NonSerialized] public int MaxMana;

    [NonSerialized] public float HealthRegen;
    [NonSerialized] public float manaRegen;

    private float hpRegenBuffer;
    private float mpRegenBuffer;
    [NonSerialized] public float attackSpeedBonus;

    [NonSerialized] public float attackSpeedBonusBonus;
    [NonSerialized] public float attackCooldown;
    [NonSerialized] public float attackDelay;
    [NonSerialized] public float lastAttackTime = float.MinValue;
    [NonSerialized] public int damage;

    [NonSerialized] public float damageBonus;
    [NonSerialized] public float doublePhysics;
    [NonSerialized] public float MoveSpeed;
    [NonSerialized] public float Radius;
    [NonSerialized] public int player = 1;
    [NonSerialized] public int alterEgoPlayer = 6;
    [NonSerialized] public float detectRange = 6f;
    [NonSerialized] public float MagicalBuffer = 0f;
    [NonSerialized] public float MagicalDebuffer = 0.90f;
    [NonSerialized] public float TrueDamage;
    [NonSerialized] public int TowerDamage;
    [NonSerialized] public int TowerAttackSpeed;
    [NonSerialized] public bool TeleportOn = false;
    [NonSerialized] public float blinkRange;     // 아이템으로 결정된 최종 도약거리
    [NonSerialized] public float blinkCooldown;
    [NonSerialized] public float blinkDuration;  // 보통 0



    [NonSerialized]public TextMeshProUGUI text;
    [NonSerialized]public ActionScript action;

    [NonSerialized]public ArmorType armorType;

    private ItemManager itemManager;
    private OriginStatFor6 originStatFor6;

    void Awake()
    {
        action = GetComponent<ActionScript>();
        itemManager = GameManager.Instance.ItemManager;

        armorType = ArmorType.패기;

        MaxHealth = 100;
        MaxMana = 100;

        MoveSpeed = 6f;
        damage = 10;
        attackDelay = 1f;
        attackCooldown = 1f;


        action = GetComponent<ActionScript>();
    }

    void Start()
    {
        text = GameManager.Instance.unitCountTexts[player -1];
        originStatFor6 = GameManager.Instance.originStatFor6;
        
    }

    void Update()
    {
        text.text = $"{GameManager.Instance.UnitCount}";
        for(int i=0;i<DataManager.targetNumberMax;i++)
        attackCooldown = attackDelay / (1 + attackSpeedBonus * 0.01f + attackSpeedBonusBonus * 0.01f);

        Animator anim = GetComponent<Animator>();
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "Attack") // 원하는 클립 이름
            {
                float animationLength = clip.length / attackCooldown;
                anim.SetFloat("AttackSpeed", animationLength);
            }
        }       

    }




    void FixedUpdate()
    {
        // 1. 매 프레임마다 누적
        int targetNum = originStatFor6.targetNumber;
        hpRegenBuffer += HealthRegen * Time.fixedDeltaTime;

        // 2. 누적값이 1 이상이면 정수만큼 회복
        if (hpRegenBuffer >= 1f)
        {
            int regenAmount = Mathf.FloorToInt(hpRegenBuffer);  // 정수만큼 회복
            CurrentHealth += regenAmount;
            CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);

            hpRegenBuffer -= regenAmount;  // 버퍼에서 소모한 만큼 빼기 (소수점 유지됨)
        }

        mpRegenBuffer += manaRegen * Time.fixedDeltaTime;

        // 2. 누적값이 1 이상이면 정수만큼 회복
        if (mpRegenBuffer >= 1f)
        {
            int regenAmount = Mathf.FloorToInt(mpRegenBuffer);  // 정수만큼 회복
            CurrentMana += regenAmount;
            CurrentMana = Mathf.Min(CurrentMana, MaxMana);

            mpRegenBuffer -= regenAmount;  // 버퍼에서 소모한 만큼 빼기 (소수점 유지됨)
        }
    }

    public void HealthTrigger(Transform target)
    {
        Actor actor = target.GetComponent<Actor>();
        if (CurrentHealth == MaxHealth)
        {
            foreach(Item item in itemManager.list.currentItem[alterEgoPlayer])
            {
                if(!item.HaveRegenSkill && item.RegenStun == 0)
                continue;
                Debug.Log("Hello");

                actor.TakeStunAll(item.RegenStun, 0, item.RegenRange);
                actor.TakeDamageAll_physics(
                    item.RegenDamage[(int)RegenKind.HealthRegen, (int)DamageKind.physics,(int)DamageTarget.MultiDamage],
                    item.RegenDamage[(int)RegenKind.HealthRegen, (int)DamageKind.physics, (int)DamageTarget.MonoDamage],
                    item.RegenRange,
                    ArmorType.일반,
                    0);
                actor.TakeDamageAll_magics(
                    item.RegenDamage[(int)RegenKind.HealthRegen, (int)DamageKind.magics,(int)DamageTarget.MultiDamage],
                    item.RegenDamage[(int)RegenKind.HealthRegen, (int)DamageKind.magics, (int)DamageTarget.MonoDamage],
                    item.RegenRange,
                    false);
                
                for(int i=0;i<(int)PercentageCategory.count;i++)
                {
                    for(int j=0;j<(int)PercentKind.count;j++)
                    {
                        actor.TakeDamageAll_percentage(
                            item.RegenPercent[(int)RegenKind.HealthRegen, i, j],
                            0,
                            item.RegenRange,
                            (PercentKind)j,
                            (PercentageCategory)i);
                    }
                }

            }
            CurrentHealth= 0;
        }
    }

    public void ManaTrigger(Transform target)
    {
        if (CurrentMana == MaxMana)
        {
            CurrentMana = 0;
        }
    }
    public Vector2 GetHP()
    {
        return new Vector2(CurrentHealth, MaxHealth);
    }
    public Vector2 GetMP()
    {
        return new Vector2(CurrentMana, MaxMana);
    }

    public (int damage, float damageBonus, float attackCooldown, float attackSpeedBonus, float HealthRegen, float manaRegen,
      float MagicalBuffer, float MagicalDebuffer, float TrueDamage, float doublePhysics, float Radius)
      GetStats()
    {
        return (damage, damageBonus, attackCooldown, attackSpeedBonus + attackSpeedBonusBonus
        , HealthRegen, manaRegen,
     MagicalBuffer, MagicalDebuffer, TrueDamage, doublePhysics, Radius);
    }

}
