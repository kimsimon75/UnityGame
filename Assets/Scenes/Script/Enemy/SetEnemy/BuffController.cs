using System.Collections.Generic;
using UnityEngine;

public enum BuffType{Slow, ArmorDecrease, Count}

public sealed class BuffController : MonoBehaviour
{
    struct Inst
    {
        public float expireAt;
        public int amount; // slowPercent: 0.2 = 20% 느려짐
    }

    readonly Dictionary<(Item item, BuffType sourceId), Inst> _debuff = new();
    readonly List<(Item item, BuffType sourceId)> _toRemove = new();
    
    EnemyStats enemyStats;

    [Range(0, 102)] int maxSlow = 102; // 합산 상한
    Actor _unit;

    void Awake()
    {
        _unit = GetComponent<Actor>();
        enemyStats = GetComponent<EnemyStats>();
    } 

    void Update()
    {
        if (_debuff.Count == 0) return;

        float now = Time.time;
        _toRemove.Clear();

        foreach (var kv in _debuff)
            if (kv.Value.expireAt <= now)
                _toRemove.Add(kv.Key);

        if (_toRemove.Count == 0) return;

        for (int i = 0; i < _toRemove.Count; i++)
            _debuff.Remove(_toRemove[i]);

        RecomputeSlowSum();
    }

    /// <summary>
    /// 같은 디버프라도 sourceId 별로 독립 유지 (강도/시간 각각)
    /// </summary>
    public void RefreshSlow(Item item, BuffType sourceId, int Amount, float duration)
    {
        float newExpire = Time.time + Mathf.Max(0f, duration);

        if (_debuff.TryGetValue((item, sourceId), out var s))
        {
            // 지속시간: 더 길게 유지되는 쪽으로
            s.expireAt = Mathf.Max(s.expireAt, newExpire);

            // 강도: "이 소스에서 최신값"으로 갱신 (원하면 Max로만 증가하게 바꿀 수 있음)
            s.amount = Amount;

            _debuff[(item, sourceId)] = s;
        }
        else
        {
            _debuff[(item, sourceId)] = new Inst { expireAt = newExpire, amount = Amount };
        }

        RecomputeSlowSum();
    }

    void RecomputeSlowSum()
    {
        int[] sum = new int[(int)BuffType.Count];
        foreach (var kv in _debuff)
        {
            sum[(int)BuffType.Slow] += kv.Value.amount;
            
        }

        float finalSlow = Mathf.Clamp(sum[(int)BuffType.Slow], 0f, maxSlow);
        float finalMove = Mathf.Clamp(enemyStats.baseMoveSpeed - Mathf.Round(3.875f * finalSlow), 0f, 70f);
        if(enemyStats != null) enemyStats.moveSpeed = finalMove;
    }
}
