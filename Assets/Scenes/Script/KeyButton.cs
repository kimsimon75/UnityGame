using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler 
{
    public int number;
    public ItemManager itemManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemManager = GameManager.Instance.ItemManager;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.KeyValueNumber = number;
    }   
    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.KeyValueNumber = DataManager.NumCount;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (number != (int)DataManager.Num.D &&
        GameManager.Instance.ItemManager.isActiveAndEnabled &&
        eventData.button == PointerEventData.InputButton.Left &&
        (Input.GetKey(KeyCode.LeftControl) ||
        Input.GetKey(KeyCode.RightControl)))
        {
            itemManager.ControlTrigger(number);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (GameManager.Instance.items.activeSelf) { itemManager.TriggerMany(number,1); }
            else
            {
                GameManager.Instance.Trigger(number);
                itemManager.GetComponent<ItemScript>().SetKey();
            }
        }
    }
}
