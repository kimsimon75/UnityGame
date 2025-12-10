using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemScrollView : MonoBehaviour
{
    public GameObject Content;
    ActionScript actionScript;
    List<GameObject> panels;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        panels = new List<GameObject>();

    }
    void Start()
    {
        actionScript = GameManager.Instance.Action;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ImageInit(PriorityQueue<Item> items)
    {
        int count = 0;
        foreach (Item item in items.EnumerateByPriority())
        {
            count += item.count;
        }
        int panelCount = panels.Count;
        if (count > panelCount)
        {
            for (int i = 0; i < count - panelCount; i++) AddImage();
        }
        else if (panels.Count > count)
        {
            for (int i = 0; i < panelCount - count; i++) RemoveImage();
        }
        SetImage(items);
    }

    public void AddImage()
    {
        GameObject panel = new GameObject("Panel", typeof(RectTransform));
        panel.AddComponent<Image>();

        panel.transform.SetParent(Content.transform, false);
        panels.Add(panel);

        UnityEngine.UI.Outline outline = panel.AddComponent<UnityEngine.UI.Outline>();
        outline.effectDistance = new Vector2(4, 4);

        GameObject itemImage = new GameObject("Item", typeof(RectTransform));
        itemImage.AddComponent<Image>();

        itemImage.transform.SetParent(panel.transform, false);

        RectTransform rt = itemImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;     // 왼쪽 아래
        rt.anchorMax = Vector2.one;      // 오른쪽 위
        rt.offsetMin = Vector2.zero;     // 여백 없음
        rt.offsetMax = Vector2.zero;
    }
    public void RemoveImage()
    {
        if (panels.Count == 0) return;

        GameObject panel = panels[panels.Count - 1];
        panels.RemoveAt(panels.Count - 1);
        Destroy(panel);

    }

    public void SetImage(PriorityQueue<Item> items)
    {
        int i = 0;
        foreach (Item item in items.EnumerateByPriority())
        {
            for (int j = 0; j < item.count; j++)
            {
                panels[i].GetComponent<UnityEngine.UI.Outline>().effectColor = GameManager.Instance.ItemManager.GetColor(item);
                panels[i++].transform.Find("Item").GetComponent<Image>().sprite = item.Resource;
            }
        }
    }
}
