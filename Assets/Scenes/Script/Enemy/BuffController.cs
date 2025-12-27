using System;
using System.Collections.Generic;
using UnityEngine;

public enum DebuffId { Slow, Poison /* ... */ }

public sealed class BuffController : MonoBehaviour
{
    struct BuffState
    {
        public float expireAt;   // Time.time 기준 만료 시각
        public int stacks;       // 필요하면
    }

    readonly Dictionary<DebuffId, BuffState> _buffs = new();

    EnemyStats _unit;

    void Awake()
    {
        _unit = GetComponent<EnemyStats>();
    }

    void Update()
    {
        if (_buffs.Count == 0) return;

        // 만료 체크
        // Dictionary 순회 중 삭제가 불가하니 임시 리스트 사용(간단 버전)
        List<DebuffId> toRemove = null;

        float now = Time.time;
        foreach (var kv in _buffs)
        {
            if (kv.Value.expireAt <= now)
            {
                (toRemove ??= new List<DebuffId>()).Add(kv.Key);
            }
        }

        if (toRemove == null) return;

        for (int i = 0; i < toRemove.Count; i++)
            Remove(toRemove[i]);
    }

    /// <summary>
    /// 이미 걸려 있으면 "만료시간 연장", 없으면 Apply 후 등록.
    /// keepAliveSeconds는 보통 tickInterval보다 살짝 길게(예: tick=0.25면 0.5~0.7)
    /// </summary>
    public void Refresh(DebuffId id, float keepAliveSeconds)
    {
        float newExpire = Time.time + keepAliveSeconds;

        if (_buffs.TryGetValue(id, out var s))
        {
            // 여러 소스가 있을 수 있으면 Max로(더 긴 만료 유지)
            s.expireAt = Mathf.Max(s.expireAt, newExpire);
            _buffs[id] = s;
            return;
        }

        // 처음 걸릴 때만 효과 적용
        Apply(id);

        s = new BuffState { expireAt = newExpire, stacks = 1 };
        _buffs.Add(id, s);
    }

    void Apply(DebuffId id)
    {
        switch (id)
        {
            case DebuffId.Slow:
                _unit.moveSpeed = 484f - 3.875f; // 예시: 30% 슬로우
                break;
            case DebuffId.Poison:
                // 독은 TickDamage 쪽에서 처리하거나 별도 시스템
                break;
        }
    }

    void Remove(DebuffId id)
    {
        if (!_buffs.Remove(id)) return;

        // 해제 시 원상복구
        switch (id)
        {
            case DebuffId.Slow:
                _unit.moveSpeed /= 0.7f;
                break;
            case DebuffId.Poison:
                break;
        }
    }

    public bool Has(DebuffId id) => _buffs.ContainsKey(id);
}
