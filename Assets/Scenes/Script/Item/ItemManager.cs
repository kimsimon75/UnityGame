using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public List list;

    public Item editItem;

    private Image[] images;
    Image[] menu;
    private Button[] buttons;
    private float blur;
    [Range(4, 128)] public int segments = 64;
    [Range(1, 256)] public int radius = 64;

    private Texture2D tex;

    private int rank = 0;
    public PlayerStats stats;
    public CannonManager cannon;

    public GameObject ItemList;
    public GameObject editItemStatus;
    public Image statusItem;
    public TextMeshProUGUI editItemName;
    public TextMeshProUGUI[] ItemStatus;
    public TextMeshProUGUI ItemExplanation;

    void Awake()
    {

        images = GetComponentsInChildren<Image>().Where(img => img.gameObject.name.ToLower().Contains("image")).ToArray();
        buttons = GetComponentsInChildren<Button>().Where(img => img.gameObject.name.ToLower().Contains("button")).ToArray();

        list = new List(stats, cannon, this);
        blur = 0.5f;

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


        foreach (Image image in images)
        {


            Image numberImage = new GameObject("number1").AddComponent<Image>();

            numberImage.transform.SetParent(image.transform);
            numberImage.transform.localPosition = new Vector3(25, -25, 0);
            numberImage.rectTransform.sizeDelta = new Vector2(15, 15);
            numberImage.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            numberImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f));

            TextMeshProUGUI text = new GameObject("text").AddComponent<TextMeshProUGUI>();
            Transform tr = text.transform;
            tr.SetParent(numberImage.transform);
            tr.localPosition = Vector3.zero;
            text.fontSize = 12;
            text.transform.localScale = new Vector3(1, 1, 1);
            text.color = Color.black;
            text.text = "";
            text.alignment = TextAlignmentOptions.Center;

            RectTransform rt = text.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            rt.pivot = new Vector2(0.5f, 0.5f);

        }

        foreach (Button button in buttons)
        {
            button.AddComponent<RightClickButtonHandler>();
        }

        menu = GetComponentsInChildren<Image>().Where(img =>
        !img.gameObject.name.ToLower().Contains("button") &&
        !img.gameObject.name.ToLower().Contains("items") &&
        !img.gameObject.name.ToLower().Contains("number1") &&
        !img.gameObject.name.ToLower().Contains("image") &&
        !img.gameObject.name.ToLower().Contains("row")).ToArray();

        foreach (Image image in menu)
        {
            image.AddComponent<MyButtonTrigger>();
        }
        list.Clear();

    }

    void Update()
    {
        if (!ItemList.activeSelf && editItemStatus.activeSelf)
        {
            statusItem.sprite = editItem.Resource;
            editItemName.text = $"아이템명 : {editItem.Name}";

            ItemStatus[0].text = $"등급 : {editItem.Rank}";
            ItemStatus[1].text = $"기본 공격력 증가 : {editItem.AttackPower}";
            ItemStatus[2].text = $"추가 공격력 : {editItem.AdditionalAttackPower}%";
            ItemStatus[3].text = $"방어력 감소 : {editItem.NeutralizeDefense}";
            ItemStatus[4].text = $"마법 증폭 : {editItem.MagicalBuffer}%";
            ItemStatus[5].text = $"마법방어력 감소 : {editItem.MagicalDebuffer}%";
            ItemStatus[6].text = $"방어무시 데미지 : {editItem.TrueDamage}%";
            ItemStatus[7].text = $"체력 재생 : {editItem.HealthRegen}";
            ItemStatus[8].text = $"마나 재생 : {editItem.ManaRegen}";
            ItemStatus[9].text = $"이동속도 감소 : {editItem.MoveSpeed}";
            ItemStatus[10].text = $"공격속도 증가 : {editItem.AttackSpeed}%";
            ItemStatus[11].text = $"타워 공격력 증가 : {editItem.TowerDamage}";
            ItemStatus[12].text = $"타워 공격속도 증가: {editItem.TowerAttackSpeed}%";

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                editItemStatus.SetActive(false);
                ItemList.SetActive(true);
                Clear(editItem);
            }

        }

    }

    public Image[] GetImages() { return images; }
    public Button[] GetButtons() { return buttons; }

    public void SetRank(int sRank)
    {
        rank = sRank;
        if (!ItemList.activeSelf && editItemStatus.activeSelf)
        {
            editItemStatus.SetActive(false);
            ItemList.SetActive(true);
        }
        Clear(null);
    }

    public int GetRank() { return rank; }

    public void Clear(Item item)
    {
        editItem = item;
        list.Clear();

        if (item == null)
        {
            string str = "row1";

            Transform[] rankMenu = {
                transform.Find($"{str}/흔함"),
                transform.Find($"{str}/안흔함"),
                transform.Find($"{str}/특별함"),
                transform.Find($"{str}/희귀함"),
                transform.Find($"{str}/전설적인"),
                transform.Find($"{str}/히든"),
                transform.Find($"{str}/변화된"),
                transform.Find($"{str}/상위"), // 7번째
            };

            foreach (Transform monoMenu in rankMenu)
            {
                monoMenu.GetComponent<Outline>().effectDistance = Vector2.zero;
            }

            rankMenu[rank].GetComponent<Outline>().effectDistance = new Vector2(4, 4);

            if (rank <= (int)ItemRank.상위 + 1)
            {
                int commonStart = (int)ItemRank.흔함;
                Item[] Items = list.itemList[rank + commonStart].ToArray();
                string s = ((ItemRank)(rank + commonStart)).ToString();
                for (int i = 0; i < list.itemList[rank + commonStart].Count; i++)
                {
                    images[i].transform.Find("number1").gameObject.SetActive(true);

                    Sprite sprite = Resources.Load<Sprite>($"Image/Item/{s}/{Items[i].Name}");
                    if (sprite == null)
                        Debug.LogError($"Sprite not found : {Items[i].Name}");
                    else
                        images[i].sprite = sprite;
                    images[i].GetComponentInChildren<TextMeshProUGUI>().text = Items[i].count.ToString();
                    Color c = images[i].color;
                    if (Items[i].count == 0) c.a = blur;
                    else c.a = 1f;
                    images[i].color = c;
                }
                if (rank == 0)
                {
                    int i = list.itemList[rank + commonStart].Count;
                    images[i].transform.Find("number1").gameObject.SetActive(true);
                    images[i].sprite = Resources.Load<Sprite>($"Image/Item/All/{list.itemList[0][0].Name}");
                    images[i].GetComponentInChildren<TextMeshProUGUI>().text = list.itemList[0][0].count.ToString();

                    Color c = images[i].color;
                    if (list.itemList[0][0].count == 0) c.a = blur;
                    else c.a = 1f;
                    images[i].color = c;

                    images[i + 1].transform.Find("number1").gameObject.SetActive(true);
                    images[i + 1].sprite = Resources.Load<Sprite>($"Image/Item/All/{list.itemList[0][1].Name}");
                    images[i + 1].GetComponentInChildren<TextMeshProUGUI>().text = list.itemList[0][1].count.ToString();

                    c = images[i + 1].color;
                    if (list.itemList[0][1].count == 0)
                    {
                        c.a = blur;
                        buttons[i].GetComponent<Outline>().effectColor = Color.red;
                    }
                    else c.a = 1f;
                    images[i + 1].color = c;
                }

            }
        }
        else
        {
            Item targetItem = item;
            List<Item> parentItems = targetItem.GetParent();
            for (int i = 0; i < parentItems.Count; i++)
            {
                images[i].sprite = parentItems[i].Resource;

                Outline line = buttons[i].GetComponent<Outline>();

                line.effectColor = GetColor(parentItems[i]);

                line.effectDistance = new Vector2(4, 4);
            }

            images[10 * 2].sprite = item.Resource;
            Outline targetItemLine = buttons[10 * 2].GetComponent<Outline>();
            targetItemLine.effectColor = GetColor(item);
            targetItemLine.effectDistance = new Vector2(4f, 4f);
            images[10 * 2 + 1].sprite = Resources.Load<Sprite>($"Image/등호");

            ItemIngredient[] ingredient = item.NecessaryItem;
            for (int i = 0; i < ingredient.Length; i++)
            {
                images[10 * 2 + 1 + 1 + i].sprite = ingredient[i].Item.Resource;
                targetItemLine = buttons[10 * 2 + 1 + 1 + i].GetComponent<Outline>();
                targetItemLine.effectDistance = new Vector2(4f, 4f);

                switch (ingredient[i].Item.Rank)
                {
                    case ItemRank.All:
                        targetItemLine.effectColor = Color.skyBlue;
                        break;
                    case ItemRank.흔함:
                        targetItemLine.effectColor = Color.green;
                        break;
                    case ItemRank.안흔함:
                        targetItemLine.effectColor = Color.purple;
                        break;
                    case ItemRank.특별함:
                        targetItemLine.effectColor = Color.yellow;
                        break;
                    case ItemRank.희귀함:
                        targetItemLine.effectColor = Color.pink;
                        break;
                    case ItemRank.전설적인:
                        targetItemLine.effectColor = Color.red;
                        break;
                    case ItemRank.히든:
                        targetItemLine.effectColor = new Color32(233, 119, 157, 255);
                        break;
                    case ItemRank.변화된:
                        targetItemLine.effectColor = new Color32(255, 0, 131, 255);
                        break;
                    case ItemRank.상위:
                        targetItemLine.effectColor = new Color32(0, 248, 153, 255);
                        break;

                }
            }
        }

    }

    public ref Item GetEditItem() { return ref editItem; }

    private Color GetColor(Item targetItem)
    {
        Dictionary<string, int> dict = list.CombineAllItem(targetItem, false);
        if (dict.ContainsKey("만물석") && list.EnoughCombine(dict))
        {
            return Color.orange;
        }
        else if (dict.ContainsKey("만물석") && !list.EnoughCombine(dict)) return Color.red;
        else return Color.blue;

    }
}
