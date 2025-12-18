using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [Header("참조")]
    public RectTransform content;      // 위 Content
    public GameObject messagePrefab;   // 위 MessagePrefab
    public Canvas rootCanvas; // 비워두면 자동으로 찾음

    int lastW, lastH;
    float lastScale;
    bool needFix;


    [Header("옵션")]
    [NonSerialized] public int maxLines = 15;          // 동시에 보이는 최대 줄 수
    public float fadeDelay = 2f;
    public float fadeDuration = 1f;

    IEnumerator FixTMPNextFrame(TMP_Text t)
    {
        // 레이아웃/CanvasScaler 확정될 때까지 한 프레임 대기
        yield return null;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);

        // TMP 메쉬/패딩 재생성 (Outline/Underlay 포함)
        t.UpdateMeshPadding();
        t.ForceMeshUpdate(true, true);
        t.SetAllDirty();
    }

    public void Push(string text)
    {
        var go = Instantiate(messagePrefab, content);
        go.transform.SetAsFirstSibling();

        var txt = go.GetComponent<TMP_Text>();
        txt.text = text;

        go.GetComponent<ChatMessageAutoHeight>().Refresh();

        // ✅ 이 한 줄이 핵심: Start에 박힌 애들도 '페이드 시작 전'에 정상화됨
        StartCoroutine(FixTMPNextFrame(txt));

        var fade = go.GetComponent<TMPFadeOut_Update>();
        fade.delay    = fadeDelay;
        fade.duration = fadeDuration;
        fade.StartFade();

        if (content.childCount > maxLines)
            Destroy(content.GetChild(content.childCount - 1).gameObject);
    }

    void Awake()
    {
        if (!rootCanvas) rootCanvas = GetComponentInParent<Canvas>();
        lastW = Screen.width;
        lastH = Screen.height;
        lastScale = rootCanvas ? rootCanvas.scaleFactor : 1f;
    }

    void OnEnable()
    {
        Canvas.willRenderCanvases += OnWillRenderCanvases;
    }

    void OnDisable()
    {
        Canvas.willRenderCanvases -= OnWillRenderCanvases;
    }

    void OnWillRenderCanvases()
    {
        float scale = rootCanvas ? rootCanvas.scaleFactor : 1f;

        // GameView 크기 변경 / CanvasScaler scaleFactor 변경 감지
        if (Screen.width != lastW || Screen.height != lastH || !Mathf.Approximately(scale, lastScale))
        {
            lastW = Screen.width;
            lastH = Screen.height;
            lastScale = scale;
            needFix = true;
            // 여기서 바로 리빌드하면 "CanvasScaler 적용 전"일 수 있어서 코루틴으로 한 박자 늦춤
            StartCoroutine(FixChatTMPNextFrame());
        }
    }

    IEnumerator FixChatTMPNextFrame()
    {
        if (!needFix) yield break;
        needFix = false;

        // ✅ CanvasScaler/레이아웃 적용이 끝난 뒤에
        yield return new WaitForEndOfFrame();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);

        foreach (var t in content.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            // ✅ TMP 머티리얼/메쉬를 강제로 다시 계산
            t.UpdateMeshPadding();
            t.ForceMeshUpdate(true, true);

            // 이 한 줄이 은근히 잘 먹힘: Graphic 리빌드 경로도 한 번 태움
            t.Rebuild(CanvasUpdate.PreRender);

            t.SetAllDirty();
        }
    }
}
