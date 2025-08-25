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
        button = GetComponentInParent<Button>();

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
        throw new System.NotImplementedException();
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

        Item editItem = item.GetEditItem();
        if (editItem != null)
            item.itemStack.Push(editItem);

        string s = img.sprite.name;
        Item findItem = item.list.FindItem(s, DataManager.Instance.imageDict[img.sprite]);

        if (editItem == findItem)
        {
            GridLayoutGroup grid = GetComponentInParent<GridLayoutGroup>();
            Transform EditItemStatus = GetComponentInParent<ItemManager>().transform.Find("Panel");
            EditItemStatus.gameObject.SetActive(true);
            grid.gameObject.SetActive(false);
            item.SetStatus();
        }
        else
            item.Clear(findItem, false);
    }
}
