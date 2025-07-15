using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPFadeOut_Update : MonoBehaviour
{
    [Header("설정")]
    public float delay = 0.0f;      // 시작 전 대기
    public float duration = 1.5f;   // 페이드 소요 시간
    public bool disableAtEnd = true;

    TextMeshProUGUI tmp;
    Color baseColor;
    float timer = 0f;
    bool fading = false;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        baseColor = tmp.color;
        
    }

    // 필요할 때 호출
    public void StartFade()
    {
        timer  = 0f;
        fading = true;
        tmp.color = baseColor;               // 알파 완전 불투명
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!fading) return;

        timer += Time.deltaTime;

        // delay 동안 대기
        if (timer < delay) return;

        // 0~1 보간 인덱스
        float t = (timer - delay) / duration;
        if (t >= 1f)
        {
            // 끝!
            tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
            fading = false;
            if (disableAtEnd) Destroy(gameObject);
            return;
        }

        float alpha = Mathf.Lerp(1f, 0f, t);
        tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
    }
}
