using System.Collections.Generic;
using UnityEngine;

public class AOEBlueHighlighter : MonoBehaviour
{
    [Header("AOE 설정")]
    [HideInInspector] public float radius = 3f;            // 원 반경
    public float height = 2f;            // 원통 전체 높이
    public LayerMask unitMask;           // 유닛 레이어
    public float tintStrength = 1f;      // 틴트 세기(네 Highlightable은 값 무시해도 OK)
    public QueryTriggerInteraction triggerMode = QueryTriggerInteraction.Collide;

    [Header("최적화")]
    public int maxUnits = 70;            // 한 프레임 최대 감지 수(버퍼 크기)

    [Header("디버그")]
    public bool logEnterExit = false;

    // 내부 상태
    Collider[] _buf;
    public readonly HashSet<Highlightable> _inside = new();
    readonly List<Highlightable> _toRemove = new();

    void Awake()
    {
        _buf = new Collider[Mathf.Max(1, maxUnits)];
    }

    void Update()
    {
        Vector3 c = transform.position;
        Vector3 up = transform.up;                    // 회전된 원통 지원
        float half = height * 0.5f;

        // 원통(캡슐) 범위 질의
        int n = Physics.OverlapCapsuleNonAlloc(
            c + up * half,
            c - up * half,
            radius,
            _buf,
            unitMask,
            triggerMode
        );

        // 이번 프레임 감지된 유닛 집합(중복 제거용)
        var now = HashSetPool<Highlightable>.Get();

        for (int i = 0; i < n; i++)
        {
            var col = _buf[i];
            if (!col) continue;

            // ⚠️ 콜라이더가 자식에 있어도 부모에 붙은 Highlightable을 잡도록
            var h = col.GetComponentInParent<Highlightable>();
            if (!h) continue;

            now.Add(h);
            if (_inside.Add(h))
            {
                if (logEnterExit) Debug.Log($"[AOE] Enter: {h.name}");
                h.SetBlueTint(tintStrength); // 네 버전은 바로 파랑 설정
            }
        }

        // 나간 유닛 정리
        _toRemove.Clear();
        foreach (var h in _inside)
        {
            if (!h || !now.Contains(h)) _toRemove.Add(h);
        }
        foreach (var h in _toRemove)
        {
            if (h) { h.ClearTint(); if (logEnterExit) Debug.Log($"[AOE] Exit : {h.name}"); }
            _inside.Remove(h);
        }

        HashSetPool<Highlightable>.Release(now);

        // 버퍼가 꽉 찼으면 경고(더 많은 유닛이 있을 수 있음)
        if (n == _buf.Length && logEnterExit)
            Debug.LogWarning("[AOE] Overlap buffer full. Consider increasing maxUnits.");
    }

    void OnDisable()
    {
        foreach (var h in _inside) if (h) h.ClearTint();
        _inside.Clear();
    }

    void OnDestroy() => OnDisable();

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // 디버그: 원통 와이어
        var c = transform.position;
        var up = transform.up;
        float half = height * 0.5f;

        UnityEditor.Handles.color = new Color(0, 0.6f, 1f, 0.35f);
        UnityEditor.Handles.DrawWireDisc(c + up * half, up, radius);
        UnityEditor.Handles.DrawWireDisc(c - up * half, up, radius);
        for (int i = 0; i < 8; i++)
        {
            float ang = i * Mathf.PI * 0.25f;
            var side = (Quaternion.AngleAxis(Mathf.Rad2Deg * ang, up) * (Vector3.right * radius));
            UnityEditor.Handles.DrawLine(c + up * half + side, c - up * half + side);
        }
    }
#endif
}

// 간단한 HashSet 풀
static class HashSetPool<T>
{
    static readonly Stack<HashSet<T>> pool = new();
    public static HashSet<T> Get() => pool.Count > 0 ? pool.Pop() : new HashSet<T>();
    public static void Release(HashSet<T> set) { if (set == null) return; set.Clear(); pool.Push(set); }
}
