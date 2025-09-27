using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int UnitCount;
    [NonSerialized] public int[] CurrentHealth;
    [NonSerialized] public int[] MaxHealth;
    [NonSerialized] public int[] CurrentMana;
    [NonSerialized] public int[] MaxMana;

    [NonSerialized] public float[] HealthRegen;
    [NonSerialized] public float[] manaRegen;

    private float[] hpRegenBuffer;
    private float[] mpRegenBuffer;
    [NonSerialized] public float attackSpeedBonus;
    [NonSerialized] public float blendingTime = 0.1f;
    [NonSerialized] public float[] attackCooldown;
    [NonSerialized] public float[] attackDelay;
    [NonSerialized] public float lastAttackTime = float.MinValue;
    [NonSerialized] public float[] damage;
    [NonSerialized] public float[] doublePhysics;
    [NonSerialized] public float MoveSpeed;
    [NonSerialized] public float[] Radius;
    [NonSerialized] public float[] DamageUp;
    public int player = 1;
    [NonSerialized] public float detectRange = 4f;

    public int neutralizeDefense = 0;
    public int MagicalBuffer;
    public int MagicalDebuffer;
    public int[] TrueDamage;
    public int MoveSpeeDebuff;
    public int TowerDamage;
    public int TowerAttackSpeed;



    public TextMeshProUGUI text;
    public ActionScript action;

    void Awake()
    {
        CurrentHealth = new int[GameManager.Instance.Action.TargetNumberMax];
        MaxHealth = new int[GameManager.Instance.Action.TargetNumberMax];
        CurrentMana = new int[GameManager.Instance.Action.TargetNumberMax];

        MaxMana = new int[GameManager.Instance.Action.TargetNumberMax];

        HealthRegen = new float[GameManager.Instance.Action.TargetNumberMax];
        manaRegen = new float[GameManager.Instance.Action.TargetNumberMax];

        hpRegenBuffer =  new float[GameManager.Instance.Action.TargetNumberMax];
        mpRegenBuffer = new float[GameManager.Instance.Action.TargetNumberMax];

        attackCooldown = new float[GameManager.Instance.Action.TargetNumberMax];
        attackDelay = new float[GameManager.Instance.Action.TargetNumberMax];

        damage = new float[GameManager.Instance.Action.TargetNumberMax];
        doublePhysics = new float[GameManager.Instance.Action.TargetNumberMax];
        DamageUp = new float[GameManager.Instance.Action.TargetNumberMax];
        Radius = new float[GameManager.Instance.Action.TargetNumberMax];

        TrueDamage = new int[GameManager.Instance.Action.TargetNumberMax];

        UnitCount = 0;
        for (int i = 0; i < GameManager.Instance.Action.TargetNumberMax; i++)
        {
            MaxHealth[i] = 100;
            CurrentHealth[i] = 0;
            
            MaxMana[i] = 100;          // ➕ 추가
            CurrentMana[i] = 0;

            HealthRegen[i] = 0f;
            manaRegen[i] = 0f;
        }        // ➕ 추가

        MoveSpeed = 6f;


        action = GetComponent<ActionScript>();

        for (int i = 0; i < damage.Length; i++)
        {
            damage[i] = 50;
            attackDelay[i] = 1f;
            attackSpeedBonus = 0f;
            attackCooldown[i] = 1f;
        }

 
    }

    void Update()
    {
        text.text = $"{UnitCount}";
        for(int i=0;i<GameManager.Instance.Action.TargetNumberMax;i++)
        attackCooldown[i] = attackDelay[i] / (1 + attackSpeedBonus * 0.01f);

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

    public (float[] damage, float attackCooldown, float attackSpeedBonus,
     int neutralizeDefense, float HealthRegen, float manaRegen,
      int MagicalBuffer, int MagicalDebuffer, int TrueDamage, int MoveSpeeDebuff, float[] doublePhysics, float[] Radius)
      GetStats()
    {
        return (damage, attackCooldown[action.targetNumber], attackSpeedBonus, neutralizeDefense
    , HealthRegen[action.targetNumber], manaRegen[action.targetNumber],
     MagicalBuffer, MagicalDebuffer, TrueDamage[action.targetNumber], MoveSpeeDebuff, doublePhysics, Radius);
    }

}
