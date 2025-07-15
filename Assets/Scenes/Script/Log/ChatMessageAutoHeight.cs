using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text), typeof(RectTransform))]
public class ChatMessageAutoHeight : MonoBehaviour
{
    public float vPadding = 0f;   // 위·아래 여백

    TMP_Text       txt;
    RectTransform  rt;

    void Awake()
    {
        txt = GetComponent<TMP_Text>();
        rt  = GetComponent<RectTransform>();
    }

    // 텍스트를 넣은 뒤 반드시 한 번 호출
    public void Refresh()
    {
        txt.ForceMeshUpdate();                     // 줄 수·크기 계산

        float h = txt.preferredHeight + vPadding;

        // 높이·폭 직접 설정
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   h);
    }
}
