using UnityEngine;

public class TargetUIFollower : MonoBehaviour
{
    private Transform target;
    public Camera mainCam;
    [SerializeField] private Vector2 screenOffsetRatio = new Vector2(0f, 0.005f); // 화면 비율 오프셋
    [SerializeField] private float extraVerticalRatio = 0.05f;

    private float cachedHeight = 2f;
    private RectTransform rt;

    void Awake()
    {
        rt = transform as RectTransform;
    }

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null || !target.gameObject.activeInHierarchy || mainCam == null)
        {
            gameObject.SetActive(false);
            return;
        }

        UpdatePosition();
    }

    public void Follow(Transform newTarget)
    {
        target = newTarget;
        if (target == null) { gameObject.SetActive(false); return; }

        cachedHeight = GetTargetHeightCached(target);   // ✅ 타겟 바뀔 때만 계산
        gameObject.SetActive(true);
        UpdatePosition();                               // ✅ LateUpdate와 같은 로직 사용
    }

    public void StopFollow()
    {
        target = null;
        gameObject.SetActive(false);
    }

    void UpdatePosition()
    {
        Vector3 worldPos = target.position + Vector3.up * cachedHeight;
        Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

        // 카메라 뒤면 숨김
        if (screenPos.z <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        // 화면 비율 기반 오프셋
        screenPos.y += Screen.height * extraVerticalRatio;
        screenPos.x += Screen.width  * screenOffsetRatio.x;
        screenPos.y += Screen.height * screenOffsetRatio.y;

        // UI면 보통 이게 더 자연스러움(Overlay 기준)
        if (rt != null) rt.position = screenPos;
        else transform.position = screenPos;
    }

static float GetTargetHeightCached(Transform t)
{
    // 1) Collider가 있으면 "로컬 기준" 값 사용 (스케일 영향 X)
    if (t.TryGetComponent<Collider>(out var col))
    {
        switch (col)
        {
            case CapsuleCollider cap: return cap.height;
            case BoxCollider box:     return box.size.y;
            case SphereCollider sph:  return sph.radius * 2f;
            case CharacterController cc: return cc.height;
        }

        // 기타 콜라이더는 로컬 값을 직접 얻기 애매하니 (임시) 스케일 제거로 처리
        float sy = Mathf.Abs(col.transform.lossyScale.y);
        if (sy < 1e-4f) sy = 1f;
        return col.bounds.size.y / sy; // 월드 bounds를 스케일로 나눠 "대충 로컬화"
    }

    // 2) Renderer면 Mesh 로컬 bounds 사용 (스케일 영향 X)
    if (t.TryGetComponent<SkinnedMeshRenderer>(out var smr) && smr.sharedMesh != null)
        return smr.sharedMesh.bounds.size.y;

    if (t.TryGetComponent<MeshFilter>(out var mf) && mf.sharedMesh != null)
        return mf.sharedMesh.bounds.size.y;

    return 2f;
}

}
