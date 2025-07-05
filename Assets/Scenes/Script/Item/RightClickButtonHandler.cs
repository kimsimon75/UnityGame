using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Collections.LowLevel.Unsafe;

public class RightClickButtonHandler : MonoBehaviour, IPointerClickHandler
{
    private ItemManager item;
    public Button myButton;
    Image[] images;
    Button[] buttons;

    void Awake()
    {
        myButton = GetComponent<Button>();
        item = GetComponentInParent<ItemManager>();
        images = item.GetImages();
        buttons = item.GetButtons(); 
    }

    void Start()
    {
        if (myButton != null)
        {
            myButton.onClick.AddListener(LeftButtonTrigger);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} 에 Button 컴포넌트가 없습니다.");
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {

        Transform imgTf = transform.Find("Image");
        if (imgTf == null)
            return;  // 자식이 없으면 종료

        // 2) 해당 오브젝트에서 Image 컴포넌트를 가져오고
        Image img = imgTf.GetComponent<Image>();
        if (img == null)
            return;  // Image 컴포넌트가 없으면 종료

        // 3) sprite가 할당되어 있는지 최종 확인
        if (img.sprite == null)
            return; 

        if (transform.Find("Image").GetComponent<Image>().sprite == null) return;
        // 마우스 오른쪽 버튼 클릭 시
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {

                Debug.Log("Ctrl + 오른쪽 클릭됨!");
                // 여기에 Ctrl+우클릭용 로직
                CtrlRightClickTrigger();
            }
            else
            {
                Debug.Log("오른쪽 클릭됨!");
                RightButtonTrigger(); // 여기에 원하는 트리거 함수 호출
            }
        }
    }
    void RightButtonTrigger()
    {
        // 원하는 행동 수행
        Debug.Log("Right-click triggered!");
        if (!item.list.CombineItem(item.list.FindItem(transform.Find("Image").GetComponent<Image>().sprite.name)))
            Debug.LogError("아이템이 모자라거나 만물석임");



        
    }
    void CtrlRightClickTrigger()
    {
        Dictionary<string, int> dict = item.list.CombineAllItem(item.list.FindItem(transform.Find("Image").GetComponent<Image>().sprite.name), true);

        foreach (KeyValuePair<string, int> kvp in dict)
        {
            Debug.Log($"{kvp.Key}, {kvp.Value}");
        }
    }

    void LeftButtonTrigger()
    {
        Transform imgTf = transform.Find("Image");
        if (imgTf == null)
            return;  // 자식이 없으면 종료

        // 2) 해당 오브젝트에서 Image 컴포넌트를 가져오고
        Image img = imgTf.GetComponent<Image>();
        if (img == null)
            return;  // Image 컴포넌트가 없으면 종료

        // 3) sprite가 할당되어 있는지 최종 확인
        if (img.sprite == null)
            return; 
            
        if (transform.Find("Image").GetComponent<Image>().sprite == null) return;
        string s = transform.Find("Image").GetComponent<Image>().sprite.name;
         

        item.Clear(item.list.FindItem(s));
    }
}
