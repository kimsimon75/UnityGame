using UnityEngine;

public class TargetUIFollower : MonoBehaviour
{
    private Transform target;
    public Camera mainCam;
    private Vector3 screenOffset = new Vector3(0, 0.005f, 0);

    void Start()
    {
        mainCam ??= Camera.main;
    }

    void LateUpdate()
{
    if (target != null)
    {
        // 1. 유닛 머리 위 월드 위치 계산
        Vector3 worldPos = target.position + Vector3.up * 2f;

        // 2. 화면 좌표로 변환
        Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);

        // 3. 화면 해상도 기반 오프셋 보정 (해상도 비례)
        float verticalOffset = Screen.height * 0.05f;
        screenPos.y += verticalOffset;

        // 4. 추가 사용자 오프셋 적용 (X, Y)
        screenPos += new Vector3(Screen.width * screenOffset.x, Screen.height * screenOffset.y, 0f);

        // 5. UI 위치 지정
        transform.position = screenPos;
    }
}

    public void Follow(Transform newTarget)
    {
        target = newTarget;
        gameObject.SetActive(true);
        SnapToTarget(); // ✅ 즉시 위치 갱신
    }

    public void StopFollow()
    {
        target = null;
        gameObject.SetActive(false);
    }

    void SnapToTarget()
    {
        if (target == null || mainCam == null) return;

        Vector3 screenPos = mainCam.WorldToScreenPoint(target.position);
        transform.position = screenPos + screenOffset;
    }
}

