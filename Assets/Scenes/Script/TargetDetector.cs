using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TargetDetector : MonoBehaviour
{
    public GameObject targetPanel; // 👈 타겟 표시용 UI 패널 (Text 포함된 부모)
    public Camera mainCam;
    public Vector3 screenOffset = new Vector3(0, 50, 0);
    public float stickyTime = 0.15f;
    float lastHitTime;

    Transform current;
    TargetUIFollower follower;
    public RaycastHit hit;

    void Awake()
    {
        // targetPanel 아래 UI들이 마우스 레이캐스트를 막지 않게
        foreach (var g in targetPanel.GetComponentsInChildren<Graphic>(true))
            g.raycastTarget = false;

        foreach (var t in targetPanel.GetComponentsInChildren<TextMeshProUGUI>(true))
            t.raycastTarget = false;
        follower = targetPanel.GetComponent<TargetUIFollower>();
    }   
    void Start()
    {
        mainCam ??= Camera.main;
        if (targetPanel != null)
            targetPanel.SetActive(false); // 시작 시 꺼두기
    }

    void Update()
    {

        
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            
            Transform hitEnemy = null;

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f)&&
    !UiRayUtil.IsPointerOverUIExcept(LayerMask.GetMask("Text")))
        {   
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                hit = hitInfo;
                hitEnemy = hit.transform;
                lastHitTime = Time.unscaledTime;
            }
        }

    // 유닛 위에 없으면 숨김
        if (hitEnemy != null)
        {
            current = hitEnemy;
            follower.Follow(current);
        }
        else
        {
            if (current != null && Time.unscaledTime - lastHitTime > stickyTime)
            {
                current = null;
                follower.StopFollow();
            }
        }
    }
}
