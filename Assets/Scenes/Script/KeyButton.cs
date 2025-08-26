using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyButton : MonoBehaviour, IPointerClickHandler
{
    ItemManager item;
    Button button;
    public int number;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        item = GameManager.Instance.item;
        button = GetComponentInParent<Button>(true);

        if (button != null)
        {
            button.onClick.AddListener(LeftButtonTrigger);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} 에 Button 컴포넌트가 없습니다.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // 디버그: 현재 포인터 아래 UI들 출력
        // 우선 여긴 비워두고, onClick(LeftButtonTrigger)로 처리
    }

    void LeftButtonTrigger()
    {
        switch (number)
        {
            case DataManager.Z:
                GameManager.Instance.TriggerZ();
                break;
            case DataManager.W:
                GameManager.Instance.TriggerW();
                break;
        }
    }
}
