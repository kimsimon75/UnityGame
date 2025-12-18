using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPFadeOut_Update : MonoBehaviour
{
    public float delay = 0f;
    public float duration = 1.5f;
    public bool disableAtEnd = true;

    TextMeshProUGUI tmp;
    Color baseColor;
    float timer;
    bool fading;

    int warmupFrames; // ✅ 딜레이 구간에 메쉬 갱신용

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        baseColor = tmp.color;
    }

    public void StartFade()
    {
        timer = 0f;
        fading = true;
        warmupFrames = 2;                 // ✅ 처음 2프레임만 강하게 갱신
        tmp.color = baseColor;            // 알파 1 시작
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!fading) return;

        timer += Time.deltaTime;

        // ✅ delay 구간에서도 TMP를 조금 갱신시켜서
        // Start에 찍힌 “이상한 초기 메쉬 상태”를 빨리 정상화
        if (timer < delay)
        {
            tmp.color = baseColor;        // 계속 불투명 유지
            tmp.SetVerticesDirty();       // 매 프레임 가볍게 갱신

            if (warmupFrames-- > 0)
            {
                tmp.UpdateMeshPadding();
                tmp.ForceMeshUpdate(true, true); // 처음 1~2프레임만 강하게
            }
            return;
        }

        float t = (timer - delay) / duration;
        if (t >= 1f)
        {
            tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
            fading = false;
            if (disableAtEnd) Destroy(gameObject);
            return;
        }

        float alpha = Mathf.Lerp(1f, 0f, t);
        tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
    }
}
