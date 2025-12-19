using UnityEngine;
// UI Outline과 이름 충돌을 확실히 피하려고 별칭을 사용
using QuickOutline = global::Outline;

public class Highlightable : MonoBehaviour
{
    [Header("디버깅")] public bool enableDebugLogs = true;

    QuickOutline _outline;
    Color _orig;

    float _currentTintStrength;
    void Awake()
    {
        if (!TryGetComponent(out _outline))
        {
            Debug.LogWarning($"{name}: QuickOutline(Outline) 없음");
            return;
        }

        // 반드시 켜져 있어야 OnEnable()에서 머티리얼이 붙음
        if (!_outline.enabled) _outline.enabled = true;

    }
    public void RemoveTintRequest()
    {
        Color red = Color.red;
        _outline.OutlineColor = red;
    }

    public void SetBlueTint(float s) { ApplyTint(); }
    public void ClearTint() { RemoveTintRequest(); }

    void ApplyTint()
    {
        if (_outline == null) return;

        // QuickOutline은 프로퍼티 세터가 needsUpdate=true로 바꿔서
        // 다음 프레임 Update()에서 UpdateMaterialProperties()가 자동 호출됨.
        Color blue   = new Color(0.2f, 0.6f, 1f, _orig.a);

        _outline.OutlineColor = blue;

        // 두께가 0으로 내려가 있었던 경우를 대비해 한 번 더 보정
        if (_currentTintStrength > 0f && _outline.OutlineWidth < 1f)
            _outline.OutlineWidth = 2f;
    }
}
