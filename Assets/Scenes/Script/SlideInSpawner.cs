using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class SlideInSpawner : MonoBehaviour
{
    public RectTransform panelA;               // 부모 패널
    public GameObject panelBPrefab;            // 프리팹
    public int targetChildIndex = 1;           // 도달할 자식 번호 (0부터 시작)
    private float stopOffsetX = 5f;            // 자식 위치에서 떨어질 거리
    public float duration = 1f;                // 애니메이션 시간

    void Start()
    {
    }

    System.Collections.IEnumerator SpawnPanelsSequentially()
    {

        RectTransform lastChild = panelA.GetChild(panelA.childCount - 1) as RectTransform;

        // 새 패널 생성
        GameObject newPanel = Instantiate(panelBPrefab, panelA);
        RectTransform rect = newPanel.GetComponent<RectTransform>();
        RectTransform ChildRect = lastChild.GetComponent<RectTransform>();

        // 시작 위치: 왼쪽 바깥
        Vector2 start = new Vector2(-Mathf.Max(panelA.rect.width - rect.rect.width - 5, ChildRect.rect.width - lastChild.GetComponent<TimerPosition>().positionX + 10), 0);

        // 도착 위치: 자식 위치에서 offset만큼 왼쪽
        Vector2 childPos = new Vector2(lastChild.GetComponent<TimerPosition>().positionX, 0);
        newPanel.GetComponent<TimerPosition>().positionX = childPos.x - ChildRect.rect.width - stopOffsetX - 10;
        Vector2 target = new Vector2(childPos.x - ChildRect.rect.width - stopOffsetX - 10, childPos.y); // Y값은 고정해도 됨

        rect.anchoredPosition = start;
        StartCoroutine(SlideIn(rect, start, target, duration));
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator SlideIn(RectTransform rect, Vector2 from, Vector2 to, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);
            rect.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        rect.anchoredPosition = to;
    }
    
    IEnumerator WaitAndDo()
{
    yield return new WaitForSeconds(1f); // 1초 기다림
    Debug.Log("1초 후 실행됨");
}
}
