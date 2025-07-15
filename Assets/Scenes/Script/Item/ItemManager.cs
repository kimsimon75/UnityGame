using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public TextMeshProUGUI ItemSkillExplanation;
    public ChatManager chat;

    public Stack<Item> itemStack = new Stack<Item>();

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
            for (int i = 1; i <= 2; i++)
            {
                Image numberImage = new GameObject($"number{i}").AddComponent<Image>();

                numberImage.transform.SetParent(image.transform);
                numberImage.transform.localPosition = new Vector3(i==1 ? 20 : -20, -20, 0);
                numberImage.rectTransform.sizeDelta = new Vector2(12, 12);
                numberImage.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                numberImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f));

                TextMeshProUGUI text = new GameObject("text").AddComponent<TextMeshProUGUI>();
                Transform tr = text.transform;
                tr.SetParent(numberImage.transform);
                tr.localPosition = Vector3.zero;
                text.fontSize = 10;
                text.transform.localScale = new Vector3(1, 1, 1);
                text.color = Color.black;
                text.text = "";
                text.alignment = TextAlignmentOptions.Center;
                text.color = i == 1 ? Color.black : Color.red;

                RectTransform rt = text.rectTransform;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = rt.offsetMax = Vector2.zero;
                rt.pivot = new Vector2(0.5f, 0.5f);
            }
        }

        foreach (Button button in buttons)
        {
            button.AddComponent<RightClickButtonHandler>();
        }

        menu = GetComponentsInChildren<Image>().Where(img =>
        !img.gameObject.name.ToLower().Contains("button") &&
        !img.gameObject.name.ToLower().Contains("items") &&
        !img.gameObject.name.ToLower().Contains("number") &&
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (itemStack.Count == 0)
                Clear(null, true);
            else
                Clear(itemStack.Pop(), true);

        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            GetRankedItem(ItemRank.흔함);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            GetRankedItem(ItemRank.특별함);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            GetRankedItem(ItemRank.희귀함);
        }
        list.FindItem("기억 조각").count = 10;
    }

    private void GetRankedItem(ItemRank rank)
    {
        int neccesary = 0;
        if (rank == ItemRank.흔함) neccesary = 1;
        else if (rank == ItemRank.특별함) neccesary = 2;
        else if (rank == ItemRank.희귀함) neccesary = 4;

        if (list.FindItem("기억 조각").count >= neccesary)
        {
            int rand = UnityEngine.Random.Range(0, 100);
            Color color = Color.black;
            switch (rank)
            {
                case ItemRank.흔함:
                case ItemRank.안흔함:
                    if (rand < 85)
                    {
                        int count = list.itemList[1].Count + list.itemList[2].Count;
                        rand = UnityEngine.Random.Range(0, count);
                        Item item;

                        if (rand >= list.itemList[1].Count)
                        {
                            item = list.itemList[2][rand - list.itemList[1].Count];
                            color = Color.purple;
                        }
                        else
                        {
                            item = list.itemList[1][rand];
                            color = Color.green;
                        }

                        item.count++;
                        string hex = UnityEngine.ColorUtility.ToHtmlStringRGB(color);
                        chat.Push($"초급 도박으로 <color=#{hex}>{item.Rank}</color> 등급의 {item.Name} 획득.");
                    }
                    else
                    {
                        chat.Push("<color=red>획득에 실패하였습니다</color>");
                    }
                    break;
                case ItemRank.특별함:
                    if (rand < 70)
                    {
                        Item item = list.GetRandomItem(ItemRank.특별함);

                        chat.Push($"중급 도박으로 <color=Yellow>{item.Rank}</color> 등급의 {item.Name} 획득.");
                    }
                    else
                        chat.Push("<color=red>획득에 실패하였습니다</color>");
                    break;
                case ItemRank.희귀함:
                    if (rand < 30)
                    {
                        Item item = list.GetRandomItem(ItemRank.희귀함);

                        chat.Push($"고급 도박으로 <color=#FF00FF>{item.Rank}</color> 등급의 {item.Name} 획득.");
                    }
                    else if (rand >= 30 && rand < 70)
                    {
                        Item item = list.GetRandomItem(ItemRank.특별함);

                        chat.Push($"고급 도박으로 <color=Yellow>{item.Rank}</color> 등급의 {item.Name} 획득.");
                    }
                    else
                        chat.Push("<color=red>획득에 실패하여 행운의 토큰을 얻습니다</color>");
                    break;
            }
        }
        else chat.Push("기억 조각이 부족합니다");
    }

    public Image[] GetImages() { return images; }
    public Button[] GetButtons() { return buttons; }

    public void SetRank(int sRank)
    {
        rank = sRank;
        itemStack.Clear();

        Clear(null, true);
    }

    public int GetRank() { return rank; }

    public void Clear(Item item, bool ClearStatus)
    {
        editItem = item;
        list.Clear();

        if (ClearStatus)
        {
            editItemStatus.SetActive(false);
            ItemList.SetActive(true);
        }

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
                        images[i].transform.Find("number2").gameObject.SetActive(true);

                        Sprite sprite = Resources.Load<Sprite>($"Image/Item/{s}/{Items[i].Name}");
                        if (sprite == null)
                            Debug.LogError($"Sprite not found : {Items[i].Name}");
                        else
                            images[i].sprite = sprite;
                        TextMeshProUGUI[] countText = images[i].GetComponentsInChildren<TextMeshProUGUI>();
                        Dictionary<string, int> dict = list.CombineAllItem(Items[i], false);
                        int all = 0;
                        foreach (KeyValuePair<string, int> kvp in dict)
                        {
                            all += Mathf.Max(0, kvp.Value - list.FindItem(kvp.Key).count);
                        }
                        countText[1].text = all.ToString();

                        countText[0].text = Items[i].count.ToString();
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
                if (item.Rank != 0)
                {
                    Dictionary<string, int> dict = list.DissolutionAll(targetItem);

                    foreach (KeyValuePair<string, int> kvp in dict)
                    {
                        Debug.Log($"{kvp.Key}, {kvp.Value}");
                    }
                    object[,] common = list.table[(int)ItemRank.흔함];
                    string[] names = Enumerable.Range(0, common.GetLength(0))   // 모든 행 인덱스
                                    .Select(i => (string)common[i, 0])
                                    .ToArray();
                    int j = 0;
                    foreach (string name in names)
                    {
                        targetItem = list.FindItem(name);
                        Transform tr = images[10 * 5 + j++].transform.Find("number1");
                        tr.gameObject.SetActive(true);
                        TextMeshProUGUI countText = tr.GetComponentInChildren<TextMeshProUGUI>();
                        if (dict.ContainsKey(name))
                        {
                            Debug.Log(name);
                            countText.text = dict[name].ToString();
                        }
                        else
                            countText.text = "0";
                    }
                    j = 0;
                    int all = 0;
                    foreach (string name in names)
                    {
                        targetItem = list.FindItem(name);
                        images[10 * 5 + j].sprite = targetItem.Resource;
                        Transform tr = images[10 * 5 + j++].transform.Find("number2");
                        tr.gameObject.SetActive(true);
                        TextMeshProUGUI countText = tr.GetComponentInChildren<TextMeshProUGUI>();
                        if (dict.ContainsKey(name))
                        {
                            Debug.Log(name);
                            int neccesary = Mathf.Max(dict[name] - targetItem.count, 0);
                            countText.text = neccesary.ToString();
                            all += neccesary;
                        }
                        else
                            countText.text = "0";
                    }

                    images[10 * 5 + j].sprite = list.FindItem("만물석").Resource;
                    Transform allTr = images[10 * 5 + j].transform.Find("number2");
                    allTr.gameObject.SetActive(true);
                    TextMeshProUGUI allCountText = allTr.GetComponentInChildren<TextMeshProUGUI>();
                    if ((int)item.Rank != 1)
                        allCountText.text = all.ToString();
                    else if ((int)item.Rank == 1)
                        allCountText.text = "1";
                    else
                        allCountText.text = "0";
                }

            }

    }

    public ref Item GetEditItem() { return ref editItem; }

    private Color GetColor(Item targetItem)
    {
        Dictionary<string, int> dict = list.CombineAllItem(targetItem, false);

        int all = 0;
        foreach (KeyValuePair<string, int> kvp in dict)
        {
            all += Mathf.Max(0, kvp.Value - list.FindItem(kvp.Key).count);
        }

        if (all != 0)
        {
            if(all > list.FindItem("만물석").count)return Color.red;
            else return Color.orange;
        }
        else return Color.blue;

    }

    public void SetStatus()
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

            StringBuilder s = new StringBuilder();

            int Percentage = editItem.Percentage;

            int MonoPhysics = editItem.MonoPhysics;
            int MultiPhysics = editItem.MultiPhysics;
            int MonoMagic = editItem.MonoMagic;
            int MultiMagic = editItem.MultiMagic;
            float MonoStun = editItem.MonoStun;
            float MultiStun = editItem.MultiStun;
            float Range = editItem.Range;
            int MonoPercent = editItem.MonoPercent;
            int EndPercent = editItem.EndPercent;
            int MaxPercent = editItem.MaxPercent;
            int CurrPercent = editItem.CurrPercent;
            int Max_CurrPercent = editItem.Max_CurrPercent;

            if (Percentage != 0)
            {
                s.AppendLine($"스킬 확률 : {Percentage}%");
                if (MonoPhysics != 0) s.AppendLine($"단일 물리 데미지 : {MonoPhysics}");
                if (MultiPhysics != 0) s.AppendLine($"범위 물리 데미지 : {MultiPhysics}");
                if (MonoMagic != 0) s.AppendLine($"단일 마법 데미지 : {MonoMagic}");
                if (MultiMagic != 0) s.AppendLine($"범위 마법 데미지 : {MultiMagic}");
                if (MonoStun != 0) s.AppendLine($"단일 스턴 : {MonoStun}초");
                if (MultiStun != 0) s.AppendLine($"범위 스턴 : {MultiStun}초");
                if (Range != 0) s.AppendLine($"스킬 범위 : {Range}");
                if (MonoPercent != 0) s.AppendLine($"단일 현재체력 비례 데미지 : {MonoPercent}%");
                if (EndPercent != 0) s.AppendLine($"단일 전체체력 비례 데미지 : {EndPercent}%");
                if (MaxPercent != 0) s.AppendLine($"범위 전체체력 비례 데미지 : {MaxPercent}%");
                if (CurrPercent != 0) s.AppendLine($"범위 현재체력 비례 데미지 : {CurrPercent}%");
                if (Max_CurrPercent != 0) s.AppendLine($"범위 잃은체력 비례 데미지 : {Max_CurrPercent}%");
            }
            else
                s.AppendLine("스킬이 없습니다");


            ItemSkillExplanation.text = s.ToString();

        }
    }
}
