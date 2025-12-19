using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class RightClickButtonHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private ItemManager item;
    public Button myButton;

    void Awake()
    {
        myButton = GetComponent<Button>();
        item = GameManager.Instance.ItemManager;
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

        // 마우스 오른쪽 버튼 클릭 시
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                // 여기에 Ctrl+우클릭용 로직
                CtrlRightClickTrigger(img);
            }
            else
            {
                RightButtonTrigger(img); // 여기에 원하는 트리거 함수 호출
            }
        }
    }
    void RightButtonTrigger(Image image)
    {
        if (!item.list.CombineItem(item.list.FindItem(image.sprite.name, DataManager.Instance.imageDict[image.sprite])))
            Debug.LogError("아이템이 모자라거나 만물석임");
    }
    void CtrlRightClickTrigger(Image image)
    {
        item.list.CombineSmart(item.list.FindItem(image.sprite.name, DataManager.Instance.imageDict[image.sprite]));
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

            
        string s = img.sprite.name;
        Item findItem = item.list.FindItem(s, DataManager.Instance.imageDict[img.sprite]);

        Item editItem = item.GetEditItem();
        if (editItem != null)
            item.itemStack.Push(editItem);

        if (editItem == findItem)
        {
            GridLayoutGroup grid = GetComponentInParent<GridLayoutGroup>();
            Transform EditItemStatus = GameManager.Instance.items.transform.Find("Panel");
            EditItemStatus.gameObject.SetActive(true);
            grid.gameObject.SetActive(false);
            item.SetStatus();
        }
        else if (editItem == null)
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if ((findItem.Name == "행운의 토큰" && findItem.Rank == ItemRank.희귀함) ||
                   ( findItem.count != 0 && findItem.NecessaryItem.Count() != 0 && findItem.Rank < ItemRank.전설적인 )
                 ||
                 (findItem.Rank == ItemRank.히든 && findItem.Name == "이브") || (findItem.Rank == ItemRank.히든 && findItem.Name == "함선"))
                {
                    GameObject Count = GameManager.Instance.Count;
                    CountScript countScript = Count.GetComponent<CountScript>();
                    countScript.image.sprite = findItem.Resource;
                    countScript.setImage = true;
                    GameManager.Instance.SetCountScript();
                }
            }
            else
                item.Clear(findItem, false);
        }
        else
            item.Clear(findItem, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        item.targetImage = eventData.pointerEnter.GetComponentInParent<Button>().gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        item.targetImage = null;
    }
}
