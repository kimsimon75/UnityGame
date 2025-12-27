using UnityEngine;

public class AoeIndicatorLite : MonoBehaviour
{
    public Transform ring;          // 1x1 Quad(또는 원형 Plane) + Unlit 머티리얼
    [HideInInspector] public float radius = 4f;
    public LayerMask groundMask = ~0;

    // NonAlloc 레이캐스트 버퍼
    private RaycastHit[] _hit = new RaycastHit[1];
    private Vector3 _lastMouse;
    private const float _snap = 0.25f; // 마우스가 이만큼 움직일 때만 갱신(최적화)

    void Update()
    {
        if (ring.gameObject.activeSelf == false) return;
        if ((Input.mousePosition - _lastMouse).sqrMagnitude < _snap * _snap && !Camera.main.transform.hasChanged)
    return;
        _lastMouse = Input.mousePosition;

        LayerMask floor = LayerMask.GetMask("Floor1Geo");
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.RaycastNonAlloc(ray, _hit, 500f, floor, QueryTriggerInteraction.Ignore) > 0)
        {
            var h = _hit[0];
            ring.position = h.point; // y를 항상 고정
            ring.localScale = new Vector3(radius * 2f , 1f, radius * 2f);
        }
    }

    public void SetRing(float radius, bool Active)
    {
        ring.gameObject.SetActive(Active);
        ring.GetComponent<AOEBlueHighlighter>().radius = radius;
        this.radius = radius;
    }
}
