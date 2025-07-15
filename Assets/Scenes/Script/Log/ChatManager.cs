using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [Header("참조")]
    public RectTransform content;      // 위 Content
    public GameObject messagePrefab;   // 위 MessagePrefab

    [Header("옵션")]
    public int maxLines = 10;          // 동시에 보이는 최대 줄 수
    public float fadeDelay = 2f;
    public float fadeDuration = 1f;

    public void Push(string text)
    {
        var go = Instantiate(messagePrefab, content);
        go.transform.SetAsFirstSibling();          // 새 메시지를 윗순서에

        var txt = go.GetComponent<TMP_Text>();
        txt.text = text;

        go.GetComponent<ChatMessageAutoHeight>().Refresh();   // ← 이거 한 줄로 크기 끝

        // 페이드
        var fade = go.GetComponent<TMPFadeOut_Update>();
        fade.delay    = fadeDelay;
        fade.duration = fadeDuration;
        fade.StartFade();

        // 최대 라인 초과 시 맨 아래 제거
        if (content.childCount > maxLines)
            Destroy(content.GetChild(content.childCount - 1).gameObject);
    }


}
