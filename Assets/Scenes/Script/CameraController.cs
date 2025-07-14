using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float borderThickness = 10f; // 마우스 감지 거리 (픽셀)
    public GameObject item;

    public Vector2 scrollLimitsX = new Vector2(-30f, 30f);
    public Vector2 scrollLimitsZ = new Vector2(-30f, 30f);

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
{
    Vector3 mousePos = Input.mousePosition;

    // ⛔ 마우스가 화면 바깥이면 무시
    if (mousePos.x < 0 || mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height)
        return;

    Vector3 pos = transform.position;

        if (!item.activeSelf)
        {

            if (Input.GetKey(KeyCode.UpArrow))
                pos += GetForwardFlat() * moveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.DownArrow))
                pos -= GetForwardFlat() * moveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftArrow))
                pos -= GetRightFlat() * moveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.RightArrow))
                pos += GetRightFlat() * moveSpeed * Time.deltaTime;
        }

    if (mousePos.x <= borderThickness)
            pos -= GetRightFlat() * moveSpeed * Time.deltaTime;
        else if (mousePos.x >= Screen.width - borderThickness)
            pos += GetRightFlat() * moveSpeed * Time.deltaTime;

    if (mousePos.y <= borderThickness)
        pos -= GetForwardFlat() * moveSpeed * Time.deltaTime;
    else if (mousePos.y >= Screen.height - borderThickness)
        pos += GetForwardFlat() * moveSpeed * Time.deltaTime;

    // 제한
    pos.x = Mathf.Clamp(pos.x, scrollLimitsX.x, scrollLimitsX.y);
    pos.z = Mathf.Clamp(pos.z, scrollLimitsZ.x, scrollLimitsZ.y);

    transform.position = pos;
}


    // 평면 이동용 Forward 벡터
    private Vector3 GetForwardFlat()
    {
        Vector3 fwd = transform.forward;
        fwd.y = 0;
        return fwd.normalized;
    }

    private Vector3 GetRightFlat()
    {
        Vector3 right = transform.right;
        right.y = 0;
        return right.normalized;
    }
}
