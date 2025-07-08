using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int UnitCount;
    private float CurrentHealth;
    private float MaxHealth;
    private float CurrentMana;
    private float MaxMana;

    [NonSerialized] public float HealthRegen;
    [NonSerialized] public float manaRegen;

    private float hpRegenBuffer = 0f;
    private float mpRegenBuffer = 0f;
    [NonSerialized] public float attackSpeedBonus;
    [NonSerialized] public float blendingTime;
    [NonSerialized] public float attackCooldown;
    [NonSerialized] public float attackDelay;
    [NonSerialized] public float lastAttackTime = float.MinValue;
    [NonSerialized] public float damage = 10f;
    [NonSerialized] public float MoveSpeed;
    public int player = 1;
    public SkillManager skill = new SkillManager();
    [NonSerialized] public float detectRange = 2f;

    public int neutralizeDefense = 0;



    public TextMeshProUGUI text;
    public ActionScript action;

    void Awake()
    {
        UnitCount = 0;
        MaxHealth = 100f;        // ➕ 추가
        CurrentHealth = 0;

        MaxMana = 100f;          // ➕ 추가
        CurrentMana = 0;

        HealthRegen = 0f;
        manaRegen = 0f;
        attackDelay = 1f;
        attackCooldown = 1f;
        attackSpeedBonus = 0f;
        blendingTime = 0.1f;
        MoveSpeed = 6f;


        action = GetComponent<ActionScript>();

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

    void Update()
    {
        text.text = $"{UnitCount}";
        attackCooldown = attackDelay / (1 + attackSpeedBonus * 0.01f);
    }


    void FixedUpdate()
    {
        // 1. 매 프레임마다 누적
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

    public void HealthTrigger()
    {
        if (CurrentHealth == MaxHealth)
        {
            CurrentHealth = 0;
        }
    }

    public void ManaTrigger()
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

    public Vector3 GetStats()
    {
        return new Vector3(damage, attackCooldown, attackSpeedBonus);
    }

}
