using UnityEngine;
using UnityEngine.EventSystems;
public class TargetDetector : MonoBehaviour
{
    public GameObject targetPanel; // 👈 타겟 표시용 UI 패널 (Text 포함된 부모)
    public Camera mainCam;
    public Vector3 screenOffset = new Vector3(0, 50, 0);

    void Start()
    {
        mainCam ??= Camera.main;
        if (targetPanel != null)
            targetPanel.SetActive(false); // 시작 시 꺼두기
    }

    void Update()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f)&&
    !UiRayUtil.IsPointerOverUIExcept(LayerMask.GetMask("Text")))
        {   
            if (hit.collider.CompareTag("Enemy"))
            {
                targetPanel.GetComponent<TargetUIFollower>().Follow(hit.transform);
                return;
            }
        }

    // 유닛 위에 없으면 숨김
            targetPanel.GetComponent<TargetUIFollower>().StopFollow();
    }
}
