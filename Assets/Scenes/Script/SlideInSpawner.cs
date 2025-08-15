using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class SlideInSpawner : MonoBehaviour
{
    public RectTransform panelA;               // 부모 패널
    public GameObject panelBPrefab;            // 프리팹
    public int targetChildIndex = 1;           // 도달할 자식 번호 (0부터 시작)
    private float stopOffsetX = 5f;            // 자식 위치에서 떨어질 거리
    public float positionX = -5f;

    void Start()
    {
    }

    public GameObject SpawnPanelsSequentially() // 
    {

        RectTransform lastChild = panelA.GetChild(panelA.childCount - 1) as RectTransform;

        // 새 패널 생성
        GameObject newPanel = Instantiate(panelBPrefab, panelA);
        RectTransform rect = newPanel.GetComponent<RectTransform>();
        RectTransform ChildRect = lastChild.GetComponent<RectTransform>();

        // 도착 위치: 자식 위치에서 offset만큼 왼쪽
        Vector2 childPos = new Vector2(positionX, 0);
        positionX = childPos.x - ChildRect.rect.width - stopOffsetX - 10;
        Vector2 target = new Vector2(childPos.x - ChildRect.rect.width - stopOffsetX - 10, childPos.y); // Y값은 고정해도 됨

        rect.anchoredPosition = target;

        return newPanel;
    }

    public void DeleteSlider(int targetNumber)
    {
        RectTransform target = panelA.GetChild(targetNumber) as RectTransform;

        for (int i = panelA.childCount - 1; i >= targetNumber; i--)
        {
            panelA.GetChild(i).GetComponent<RectTransform>().localPosition = panelA.GetChild(i - 1).GetComponent<RectTransform>().localPosition;
        }

        Destroy(target.gameObject);
    }
}
