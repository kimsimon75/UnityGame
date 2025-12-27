using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemScrollView : MonoBehaviour
{
    public GameObject Content;
    ActionScript actionScript;
    List<GameObject> panels;
    List<TextMeshProUGUI> texts;

    private Texture2D tex;

    [Range(1, 256)] public int radius = 64;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Init()
    {
        panels = new List<GameObject>();
        texts = new List<TextMeshProUGUI>();

        tex = new Texture2D((int)radius * 2, (int)radius * 2, TextureFormat.ARGB32, false);
        tex.SetPixels32(new Color32[(int)radius * 2 * (int)radius * 2]); // clear alpha
        for (int y = 0; y < tex.height; y++)
            for (int x = 0; x < tex.width; x++)
            {
                float dx = x - radius, dy = y - radius;
                if (dx * dx + dy * dy <= radius * radius)
                    tex.SetPixel(x, y, Color.white);
            }
        tex.Apply();
    }
    void Awake()
    {


    }
    void Start()
    {
        actionScript = GameManager.Instance.action;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ImageInit(PriorityQueue<Item> items, bool ImageSet)
    {
        int count = items.Count;
        int panelCount = panels.Count;
        if (count > panelCount)
        {
            for (int i = 0; i < count - panelCount; i++) AddImage();
        }
        else if (panels.Count > count)
        {
            for (int i = 0; i < panelCount - count; i++) RemoveImage();
        }
        SetImage(items, ImageSet);
    }

    private void AddImage()
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

        Image numberImage = new GameObject($"number").AddComponent<Image>();

        numberImage.transform.SetParent(panel.transform);
        numberImage.transform.localPosition = new Vector3(20, -20, 0);
        numberImage.rectTransform.sizeDelta = new Vector2(12, 12);
        numberImage.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        numberImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f));
        numberImage.raycastTarget = false;

        TextMeshProUGUI text = new GameObject("text").AddComponent<TextMeshProUGUI>();
        Transform tr = text.transform;
        tr.SetParent(numberImage.transform);
        tr.localPosition = Vector3.zero;
        text.fontSize = 10;
        text.transform.localScale = new Vector3(1, 1, 1);
        text.color = Color.black;
    
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black;
        text.raycastTarget = false;
        texts.Add(text);

        rt = text.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
    }
    private void RemoveImage()
    {
        if (panels.Count == 0) return;

        int last = panels.Count - 1;

        // texts도 같이 제거
        if (texts.Count > last)
            texts.RemoveAt(last);

        GameObject panel = panels[last];
        panels.RemoveAt(last);
        Destroy(panel);
    }

    private void SetImage(PriorityQueue<Item> items, bool ImageSet)
    {
        int i = 0;
        foreach (Item item in items.EnumerateByPriority())
        {
            texts[i].text = item.count.ToString();

            if(ImageSet)
            {
                panels[i].GetComponent<UnityEngine.UI.Outline>().effectColor = GameManager.Instance.ItemManager.GetColor(item);
                panels[i].transform.Find("Item").GetComponent<Image>().sprite = item.Resource;
            }

            i++;
        }
    }
}
