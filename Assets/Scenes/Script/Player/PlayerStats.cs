using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int UnitCount;
    [NonSerialized] public float[] CurrentHealth = new float[ActionScript.targetNumberMax];
    [NonSerialized] public float[] MaxHealth = new float[ActionScript.targetNumberMax];
    [NonSerialized] public float[] CurrentMana = new float[ActionScript.targetNumberMax];
    [NonSerialized] public float[] MaxMana = new float[ActionScript.targetNumberMax];

    [NonSerialized] public float[] HealthRegen = new float[ActionScript.targetNumberMax];
    [NonSerialized] public float[] manaRegen = new float[ActionScript.targetNumberMax];

    private float[] hpRegenBuffer =  new float[ActionScript.targetNumberMax];
    private float[] mpRegenBuffer = new float[ActionScript.targetNumberMax];
    [NonSerialized] public float[] attackSpeedBonus = new float[ActionScript.targetNumberMax];
    [NonSerialized] public float blendingTime = 0.1f;
    [NonSerialized] public float[] attackCooldown = new float[ActionScript.targetNumberMax];
    [NonSerialized] public float[] attackDelay = new float[ActionScript.targetNumberMax];
    [NonSerialized] public float lastAttackTime = float.MinValue;
    [NonSerialized] public float[] damage = new float[ActionScript.targetNumberMax];
    [NonSerialized] public float MoveSpeed;
    public int player = 1;
    [NonSerialized] public float detectRange = 4f;

    public int neutralizeDefense = 0;
    public int MagicalBuffer;
    public int MagicalDebuffer;
    private int[] TrueDamage = new int[ActionScript.targetNumberMax];
    public int MoveSpeeDebuff;
    public int TowerDamage;
    public int TowerAttackSpeed;



    public TextMeshProUGUI text;
    public ActionScript action;

    void Awake()
    {
        UnitCount = 0;
        for (int i = 0; i < ActionScript.targetNumberMax; i++)
        {
            MaxHealth[i] = 100f;
            CurrentHealth[i] = 0;
            
            MaxMana[i] = 100f;          // ➕ 추가
            CurrentMana[i] = 0;

            HealthRegen[i] = 0f;
            manaRegen[i] = 0f;
        }        // ➕ 추가

        MoveSpeed = 6f;


        action = GetComponent<ActionScript>();

        for (int i = 0; i < damage.Length; i++)
        {
            damage[i] = 10;
            attackDelay[i] = 1f;
            attackSpeedBonus[i] = 0f;
            attackCooldown[i] = 1f;
        }

 
    }

    void Update()
    {
        text.text = $"{UnitCount}";
        attackCooldown[action.targetNumber] = attackDelay[action.targetNumber] / (1 + attackSpeedBonus[action.targetNumber] * 0.01f);

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

    public void HealthTrigger()
    {
        int targetNum = action.targetNumber;
        if (CurrentHealth[targetNum] == MaxHealth[targetNum])
        {
            CurrentHealth[targetNum] = 0;
        }
    }

    public void ManaTrigger()
    {
        int targetNum = action.targetNumber;
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

    public (float[] damage, float attackCooldown, float[] attackSpeedBonus,
     int neutralizeDefense, float HealthRegen, float manaRegen,
      int MagicalBuffer, int MagicalDebuffer, int TrueDamage, int MoveSpeeDebuff)
      GetStats()
    {
        return (damage, attackCooldown[action.targetNumber], attackSpeedBonus, neutralizeDefense
    , HealthRegen[action.targetNumber], manaRegen[action.targetNumber],
     MagicalBuffer, MagicalDebuffer, TrueDamage[action.targetNumber], MoveSpeeDebuff);
    }

}
