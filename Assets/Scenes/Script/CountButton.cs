using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CountButton : MonoBehaviour, IPointerClickHandler
{
    public Slider slider;
    public CountScript script;
    int number;
    ItemManager itemManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        number = script.number;

        itemManager = GameManager.Instance.item;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Insert();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Insert();
    }

    private void Insert()
    {
        for (int i = 0; i < slider.value; i++)
        {
            itemManager.Trigger(number);
        }
        script.gameObject.SetActive(false);
    }
}
