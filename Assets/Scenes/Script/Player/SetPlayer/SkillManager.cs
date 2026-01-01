using UnityEngine;

public enum SkillId { Blink, Awakening, Doping, blank1, blank2, blank3 } //순서대로 q, w, e
public enum SkillScope { PerUnit, SharedAll }
public enum SkillTarget { CasterOnly, AllUnits }

public struct SkillDef
{
    public SkillId id;
    public SkillScope scope;      // 상태(쿨/지속) 저장 범위
    public SkillTarget target;    // 효과 적용 대상

    public float cooldown;
    public float duration;
    public float effect;
}

public struct SkillState
{
    public float activeUntil;
    public float cooldownUntil;

    public bool wasActive;     // 이전 프레임 활성 여부
    public int ownerIndex;     // SharedAll일 때 "누가 켰는지" 기록(시전자만 효과면 필요)

    public bool IsActive(float now) => now < activeUntil;
    public bool CanUse(float now) => now >= cooldownUntil;
}

public class SkillManager : MonoBehaviour
{
    public SkillDef[] defs;        // SkillId 개수만큼
    public SkillDef[] perDefs;
    SkillState[,] perUnit;         // [unitIndex, skillId]
    SkillState[] shared;           // [skillId]
    int unitCount;

    public void Init(int unitCount)
    {
        this.unitCount = unitCount;

        int skillCount = System.Enum.GetValues(typeof(SkillId)).Length;
        if (defs == null || defs.Length != skillCount)
            defs = new SkillDef[skillCount];

        perUnit = new SkillState[unitCount, skillCount];
        shared  = new SkillState[skillCount];

        // ownerIndex 기본값 -1
        for (int s = 0; s < skillCount; s++)
            shared[s].ownerIndex = -1;

        // ✅ 여기서 네 룰대로 정의 세팅
        defs[(int)SkillId.Awakening] = new SkillDef {
            id = SkillId.Awakening,
            scope = SkillScope.SharedAll,        // 6명 공용 쿨/지속
            target = SkillTarget.AllUnits,       // 6명 모두 효과
            cooldown = 50f,
            duration = 7f,
            effect = 80f
        };

        defs[(int)SkillId.Doping] = new SkillDef {
            id = SkillId.Doping,
            scope = SkillScope.SharedAll,
            target = SkillTarget.CasterOnly,     // 예: 공용 쿨이지만 누른 애만 효과
            cooldown = 40f,
            duration = 15f,
            effect = 15f
        };

        defs[(int)SkillId.Blink] = new SkillDef {
            id = SkillId.Blink,
            scope = SkillScope.PerUnit,          // 각자 쿨
            target = SkillTarget.CasterOnly,
            cooldown = 4f,
            duration = 0f,                       // 즉발
            effect = 0f
        };
    }

    // -------------------------
    // 조회 API (UI에서 사용)
    // -------------------------
    public float CoolLeft(int casterIndex, SkillId id)
    {
        float now = Time.time;
        ref SkillState st = ref GetState(casterIndex, id);
        return Mathf.Max(0f, st.cooldownUntil - now);
    }

    public float ActiveLeft(int casterIndex, SkillId id)
    {
        float now = Time.time;
        ref SkillState st = ref GetState(casterIndex, id);
        return Mathf.Max(0f, st.activeUntil - now);
    }

    public bool CanUse(int casterIndex, SkillId id) => CoolLeft(casterIndex, id) <= 0f;

    // -------------------------
    // 핵심: 사용
    // -------------------------
    public bool TryUse(int casterIndex, SkillId id)
    {
        float now = Time.time;
        int s = (int)id;
        SkillDef def = defs[s];

        ref SkillState st = ref GetState(casterIndex, id);

        // 쿨이면 실패
        if (!st.CanUse(now)) return false;

        // 지속형인데 이미 켜져있으면 "스택" 방지: 여기선 '갱신'만(원하면 false로 막아도 됨)
        bool alreadyActive = def.duration > 0f && st.IsActive(now);

        st.ownerIndex = casterIndex;                 // 누가 눌렀는지 기록 (Shared + CasterOnly 대비)
        st.cooldownUntil = now + def.cooldown;

        if (def.duration > 0f)
        {
            st.activeUntil = now + def.duration;

            // 처음 켜질 때만 Start 적용
            if (!alreadyActive)
            {
                ApplyEffectStart(casterIndex, id, def.effect);
                st.wasActive = true;
            }
        }
        else
        {
            // 즉발 스킬
            st.activeUntil = now; // 활성 상태 없음
            ApplyEffectInstant(casterIndex, id, def.effect);
        }

        return true;
    }

    // -------------------------
    // 지속 끝나면 자동 해제
    // -------------------------
    void Update()
    {
        float now = Time.time;
        int skillCount = defs.Length;

        // Shared 스킬 만료 체크
        for (int s = 0; s < skillCount; s++)
        {
            if (defs[s].scope != SkillScope.SharedAll) continue;
            if (defs[s].duration <= 0f) continue;

            ref SkillState st = ref shared[s];

            bool activeNow = st.IsActive(now);
            if (st.wasActive && !activeNow)
            {
                ApplyEffectEnd(st.ownerIndex, (SkillId)s, defs[s].effect);
                st.wasActive = false;
            }
        }

        // PerUnit 스킬 만료 체크
        for (int u = 0; u < unitCount; u++)
        for (int s = 0; s < skillCount; s++)
        {
            if (defs[s].scope != SkillScope.PerUnit) continue;
            if (defs[s].duration <= 0f) continue;

            ref SkillState st = ref perUnit[u, s];

            bool activeNow = st.IsActive(now);
            if (st.wasActive && !activeNow)
            {
                ApplyEffectEnd(u, (SkillId)s, defs[s].effect);
                st.wasActive = false;
            }
        }
    }

    // casterIndex가 누르더라도 scope에 따라 shared/perUnit 상태 반환
    ref SkillState GetState(int casterIndex, SkillId id)
    {
        int s = (int)id;
        if (defs[s].scope == SkillScope.SharedAll)
            return ref shared[s];
        return ref perUnit[casterIndex, s];
    }

    // -------------------------
    // 효과 적용/해제 구현부 (여기만 네 게임에 맞게 채우면 됨)
    // -------------------------
    void ApplyEffectStart(int casterIndex, SkillId id, float effect)
    {
        SkillDef def = defs[(int)id];

        if (def.target == SkillTarget.AllUnits)
        {
            // 6명 전체 버프 ON
            // foreach (var p in GameManager.Instance.player) p.GetComponent<PlayerStats>().Atk += effect;
            if(def.id == SkillId.Doping)
                foreach(PlayerStats playerStats in GameManager.Instance.playerStats)
                {
                    playerStats.damageBonus += effect;
                }
        }
        else
        {
            if (casterIndex < 0) return; // 안전
            // caster 1명 버프 ON
            // GameManager.Instance.player[casterIndex].GetComponent<PlayerStats>().Atk += effect;

            if(def.id == SkillId.Awakening)
                GameManager.Instance.playerStats[casterIndex].attackSpeedBonusBonus += effect;
        }
    }

    void ApplyEffectEnd(int casterIndex, SkillId id, float effect)
    {
        // Start에서 더했던 걸 되돌리는 방식(가장 단순)
        SkillDef def = defs[(int)id];

        if (def.target == SkillTarget.AllUnits)
        {
            // 6명 전체 버프 OFF
            // foreach (var p in GameManager.Instance.player) p.GetComponent<PlayerStats>().Atk -= effect;
            if(def.id == SkillId.Doping)
                foreach(PlayerStats playerStats in GameManager.Instance.playerStats)
                {
                    playerStats.damageBonus -= effect;
                }
        }
        else
        {
            if (casterIndex < 0) return; // 안전
            // caster 1명 버프 OFF
            // GameManager.Instance.player[casterIndex].GetComponent<PlayerStats>().Atk -= effect;
            if(def.id == SkillId.Awakening)
                GameManager.Instance.playerStats[casterIndex].attackSpeedBonusBonus -= effect;
        }
    }

    void ApplyEffectInstant(int casterIndex, SkillId id, float effect)
    {
        // 즉발(예: Blink 텔포) 처리
        // if (id == SkillId.Blink) ...

        
    }

    public float ResolveBlinkRange(int casterIndex)
    {
        return defs[(int)SkillId.Blink].effect + GameManager.Instance.playerStats[casterIndex].blinkRange;
    }
}
