using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [NonSerialized] public int UnitCount;
    [NonSerialized] public int[] CurrentHealth;
    [NonSerialized] public int[] MaxHealth;
    [NonSerialized] public int[] CurrentMana;
    [NonSerialized] public int[] MaxMana;

    [NonSerialized] public float[] HealthRegen;
    [NonSerialized] public float[] manaRegen;

    private float[] hpRegenBuffer;
    private float[] mpRegenBuffer;
    [NonSerialized] public float[] attackSpeedBonus;

    [NonSerialized] public float[] attackSpeedBonusBonus;
    [NonSerialized] public float blendingTime = 0.1f;
    [NonSerialized] public float[] attackCooldown;
    [NonSerialized] public float[] attackDelay;
    [NonSerialized] public float lastAttackTime = float.MinValue;
    [NonSerialized] public int[] damage;

    [NonSerialized] public float[] damageBonus;
    [NonSerialized] public float[] doublePhysics;
    [NonSerialized] public float MoveSpeed;
    [NonSerialized] public float[] Radius;
    [NonSerialized] public int player = 1;
    [NonSerialized] public float detectRange = 6f;

    [NonSerialized]public int neutralizeDefense = 0;
    [NonSerialized]public int MagicalBuffer;
    [NonSerialized]public int MagicalDebuffer;
    [NonSerialized]public float[] TrueDamage;
    [NonSerialized]public int MoveSpeeDebuff;
    [NonSerialized]public int TowerDamage;
    [NonSerialized]public int TowerAttackSpeed;

    [NonSerialized]public float[] someSortOfSkillActive;

    [NonSerialized]public int[] someSortOfSkillDuration;

    [NonSerialized]public float[] someSortOfSkillCooltime;

    [NonSerialized]public SkillCool[] someSortOfSkillCooldown;

    [NonSerialized]public float[] someSortOfSkillEffect;



    [NonSerialized]public TextMeshProUGUI text;
    [NonSerialized]public ActionScript action;

    [NonSerialized]public ArmorType armorType;

    void Awake()
    {
        action = GameManager.Instance.Action;

        CurrentHealth = new int[action.TargetNumberMax];
        MaxHealth = new int[action.TargetNumberMax];
        CurrentMana = new int[action.TargetNumberMax];

        MaxMana = new int[action.TargetNumberMax];

        HealthRegen = new float[action.TargetNumberMax];
        manaRegen = new float[action.TargetNumberMax];

        hpRegenBuffer =  new float[action.TargetNumberMax];
        mpRegenBuffer = new float[action.TargetNumberMax];

        attackCooldown = new float[action.TargetNumberMax];
        attackDelay = new float[action.TargetNumberMax];

        damage = new int[action.TargetNumberMax];
        doublePhysics = new float[action.TargetNumberMax];
        Radius = new float[action.TargetNumberMax];

        TrueDamage = new float[action.TargetNumberMax];

        someSortOfSkillActive = new float[DataManager.NumCount-1];
        someSortOfSkillDuration = new int[DataManager.NumCount-1];
        someSortOfSkillCooltime = new float[DataManager.NumCount-1];
        someSortOfSkillCooldown = new SkillCool[DataManager.NumCount-1];
        someSortOfSkillEffect = new float[DataManager.NumCount-1];

        attackSpeedBonus = new float[action.TargetNumberMax];

        attackSpeedBonusBonus = new float[action.TargetNumberMax];
        damageBonus = new float[action.TargetNumberMax];

        armorType = ArmorType.패기;

        UnitCount = 0;
        for (int i = 0; i < action.TargetNumberMax; i++)
        {
            MaxHealth[i] = 100;
            CurrentHealth[i] = 0;
            
            MaxMana[i] = 100;          // ➕ 추가
            CurrentMana[i] = 0;

            HealthRegen[i] = 0f;
            manaRegen[i] = 0f;
        }        // ➕ 추가

        for(int i=0;i<DataManager.NumCount-1; i++)
        {
            someSortOfSkillCooldown[i] = new SkillCool();
        }


        someSortOfSkillDuration[1] = 7;
        someSortOfSkillDuration[2] = 15;

        someSortOfSkillCooltime[0] = 4; // 도약
        someSortOfSkillCooltime[1] = 50; // 각성
        someSortOfSkillCooltime[2] = 40; // 도핑

        someSortOfSkillEffect[0] = 0f;
        someSortOfSkillEffect[1] = 80f;
        someSortOfSkillEffect[2] = 15f;


        MoveSpeed = 6f;


        action = GetComponent<ActionScript>();

        for (int i = 0; i < damage.Length; i++)
        {
            damage[i] = 10;
            attackDelay[i] = 1f;
            attackSpeedBonus[i] = 0f;
            attackCooldown[i] = 1f;
        }

        text = GameManager.Instance.unitCountTexts[player -1];
    }

    void Update()
    {
        text.text = $"{UnitCount}";
        for(int i=0;i<action.TargetNumberMax;i++)
        attackCooldown[i] = attackDelay[i] / (1 + attackSpeedBonus[i] * 0.01f + attackSpeedBonusBonus[i] * 0.01f);

        Animator anim = GetComponent<Animator>();
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "Attack") // 원하는 클립 이름
            {
                float animationLength = clip.length / attackCooldown[action.targetNumber];
                anim.SetFloat("AttackSpeed", animationLength);
            }
        }       

    }




    void FixedUpdate()
    {
        // 1. 매 프레임마다 누적
        int targetNum = action.targetNumber;
        hpRegenBuffer[targetNum] += HealthRegen[targetNum] * Time.fixedDeltaTime;

        // 2. 누적값이 1 이상이면 정수만큼 회복
        if (hpRegenBuffer[targetNum] >= 1f)
        {
            int regenAmount = Mathf.FloorToInt(hpRegenBuffer[targetNum]);  // 정수만큼 회복
            CurrentHealth[targetNum] += regenAmount;
            CurrentHealth[targetNum] = Mathf.Min(CurrentHealth[targetNum], MaxHealth[targetNum]);

            hpRegenBuffer[targetNum] -= regenAmount;  // 버퍼에서 소모한 만큼 빼기 (소수점 유지됨)
        }

        mpRegenBuffer[targetNum] += manaRegen[targetNum] * Time.fixedDeltaTime;

        // 2. 누적값이 1 이상이면 정수만큼 회복
        if (mpRegenBuffer[targetNum] >= 1f)
        {
            int regenAmount = Mathf.FloorToInt(mpRegenBuffer[targetNum]);  // 정수만큼 회복
            CurrentMana[targetNum] += regenAmount;
            CurrentMana[targetNum] = Mathf.Min(CurrentMana[targetNum], MaxMana[targetNum]);

            mpRegenBuffer[targetNum] -= regenAmount;  // 버퍼에서 소모한 만큼 빼기 (소수점 유지됨)
        }
    }

    public void HealthTrigger(int targetNum)
    {
        if (CurrentHealth[targetNum] == MaxHealth[targetNum])
        {
            CurrentHealth[targetNum] = 0;
        }
    }

    public void ManaTrigger(int targetNum)
    {
        if (CurrentMana[targetNum] == MaxMana[targetNum])
        {
            CurrentMana[targetNum] = 0;
        }
    }
    public Vector2 GetHP()
    {
        return new Vector2(CurrentHealth[action.targetNumber], MaxHealth[action.targetNumber]);
    }
    public Vector2 GetMP()
    {
        return new Vector2(CurrentMana[action.targetNumber], MaxMana[action.targetNumber]);
    }

    public (int[] damage, float[] damageBonus, float attackCooldown, float attackSpeedBonus,
     int neutralizeDefense, float HealthRegen, float manaRegen,
      int MagicalBuffer, int MagicalDebuffer, float TrueDamage, int MoveSpeeDebuff, float[] doublePhysics, float[] Radius)
      GetStats()
    {
        return (damage, damageBonus, attackCooldown[action.targetNumber], attackSpeedBonus[action.targetNumber] + attackSpeedBonusBonus[action.targetNumber]
        , neutralizeDefense, HealthRegen[action.targetNumber], manaRegen[action.targetNumber],
     MagicalBuffer, MagicalDebuffer, TrueDamage[action.targetNumber], MoveSpeeDebuff, doublePhysics, Radius);
    }

}
