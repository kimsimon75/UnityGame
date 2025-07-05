using UnityEngine;
using UnityEngine.UI;

public class MyButtonTrigger : MonoBehaviour
{
    public Button myButton;
    public ItemManager item;

    void Awake()
    {
        myButton = GetComponent<Button>();
        item = GetComponentInParent<ItemManager>();
    }
    void Start()
    {
        myButton = GetComponent<Button>();
        if (myButton != null)
        {
            myButton.onClick.AddListener(TriggerSomething);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} 에 Button 컴포넌트가 없습니다.");
        }
    }

    void TriggerSomething()
    {
        int rank = (int)(ItemRank)System.Enum.Parse(typeof(ItemRank), gameObject.name);
        item.SetRank(rank - 1);
      
        // 여기에 원하는 로직 추가
    }
}