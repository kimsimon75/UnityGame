using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.Profiling;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static MyMathf;

public class ItemManager : MonoBehaviour
{
    enum SetSoul
    {
        Z = 1 << 0,
        X = 1 << 1,
        C = 1 << 2,
    }
    public List list;

    public Item editItem;

    private Image[] images;
    private Image[] menu;
    private Button[] buttons;
    private UnityEngine.UI.Outline[] outlines;

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
    [NonSerialized] public bool isAllToggle = true;

    [NonSerialized] public float SetSoulParts = 1;

    public int willBeGet = -1;

    public Stack<Item> itemStack = new Stack<Item>();

    void Awake()
    {

        images = GetComponentsInChildren<Image>().Where(img => img.gameObject.name.ToLower().Contains("image")).ToArray();
        buttons = GetComponentsInChildren<Button>().Where(img => img.gameObject.name.ToLower().Contains("button")).ToArray();
        outlines = buttons
            .Select(b => b.GetComponent<UnityEngine.UI.Outline>())
            .Where(o => o != null)
            .ToArray();

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
            if (itemStack.Count == 0) Clear(null, true);
            else Clear(itemStack.Pop(), true);
            return;
        }

        // 2) Ctrl 상태 캐시
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        // 3) 키별로 Ctrl 조합/일반 동작 분기
        if (Input.GetKeyDown(KeyCode.Q)) { if (ctrl) ControlTrigger((int)DataManager.Num.Q); else Trigger((int)DataManager.Num.Q); return; }
        if (Input.GetKeyDown(KeyCode.W)) { if (ctrl) ControlTrigger((int)DataManager.Num.W); else Trigger((int)DataManager.Num.W); return; }
        if (Input.GetKeyDown(KeyCode.E)) { if (ctrl) ControlTrigger((int)DataManager.Num.E); else Trigger((int)DataManager.Num.E); return; }
        if (Input.GetKeyDown(KeyCode.D)) { Trigger((int)DataManager.Num.D); return; }
        if (Input.GetKeyDown(KeyCode.Z)) { if (ctrl) ControlTrigger((int)DataManager.Num.Z); else Trigger((int)DataManager.Num.Z); return; }
        if (Input.GetKeyDown(KeyCode.X)) { if (ctrl) ControlTrigger((int)DataManager.Num.X); else Trigger((int)DataManager.Num.X); return; }
        if (Input.GetKeyDown(KeyCode.C)) { if (ctrl) ControlTrigger((int)DataManager.Num.C); else Trigger((int)DataManager.Num.C); return; }
    }
    
    public void Trigger(int target)
    { if (GameManager.Instance.Count.activeInHierarchy) return;
        switch (target)
        {
            case 0:
                GetRankedItem(ItemRank.흔함);
                break;
            case 1:
                GetRankedItem(ItemRank.특별함);
                break;
            case 2:
                GetRankedItem(ItemRank.희귀함);
                break;
            case 3:
            case 4:
            case 5:
                if (isAllToggle)
                {
                    SetSoulParts = 1 << (target - (int)DataManager.Num.Z);
                    SetSouls(true);
                }
                else
                {
                    Item item = list.FindItem("영혼 파편", ItemRank.All);
                    if (item.count > 0)
                    {
                        item.count -= 1;
                        switch (target)
                        {
                            case 3:
                                list.GetRandomItem(ItemRank.흔함);
                                break;
                            case 4:
                                int rand = UnityEngine.Random.Range(0, 100);
                                if (rand < 66)
                                    list.GetMemoriesParts(1);
                                else
                                    chat.Push($"<color=Yellow>기억 조각</color> 획득에 실패했습니다.");
                                break;
                        }
                    }
                    else
                    {
                        chat.Push($"영혼 파편이 없습니다.");
                    }
                }

                break;
            case 6:
                isAllToggle = !isAllToggle;
                if(isAllToggle == true ) list.ChangeSouls();
                GameManager.Instance.Images[(int)DataManager.Num.D].GetComponent<UnityEngine.UI.Outline>().enabled = isAllToggle;
                SetSouls(isAllToggle);
                break;
        }
    }

    public void ControlTrigger(int target)
    {
            GameObject Count = GameManager.Instance.Count;

            CountScript script = Count.GetComponent<CountScript>();

            script.SetNumber(target);
            script.slider.value = 0;
            Count.SetActive(true);
    }

    private void SetSouls(bool Set)
    {
        for (int i = (int)DataManager.Num.Z; i <= (int)DataManager.Num.C; i++)
        {
            GameManager.Instance.Images[i].GetComponent<UnityEngine.UI.Outline>().enabled = false;
        }
        if (Set == false) return;
        GameManager.Instance.Images[(int)Log2(SetSoulParts) + (int)DataManager.Num.Z].GetComponent<UnityEngine.UI.Outline>().enabled = true;
        GameManager.Instance.Images[(int)DataManager.Num.D].GetComponent<UnityEngine.UI.Outline>().enabled = isAllToggle;
    }

    private void GetRankedItem(ItemRank rank)
    {
        int neccesary = 0;
        if (rank == ItemRank.흔함) neccesary = 1;
        if (rank == ItemRank.안흔함) neccesary = 1;
        else if (rank == ItemRank.특별함) neccesary = 2;
        else if (rank == ItemRank.희귀함) neccesary = 4;

        if (list.FindItem("기억 조각", ItemRank.All).count >= neccesary)
        {
            int rand = UnityEngine.Random.Range(0, 100);
            Color color = Color.black;
            switch (rank)
            {
                case ItemRank.흔함:
                case ItemRank.안흔함:
                    if (rand < 85)
                    {
                        rand = UnityEngine.Random.Range(0, 100);
                        string hex = UnityEngine.ColorUtility.ToHtmlStringRGB(Color.skyBlue);
                        if (rand < 2)
                        {
                            SetUpState(list.FindItem("함선", ItemRank.히든));
                            chat.Push($"<color=#{hex}>히든</color> 등급의 함선 획득.");
                            return;
                        }

                        int count = list.itemList[1].Count + list.itemList[2].Count;
                        rand = UnityEngine.Random.Range(0, 100);
                        Item item;

                        if (rand < 50)
                        {
                            item = list.GetRandomItem(ItemRank.안흔함, false);
                            color = Color.purple;
                        }
                        else
                        {
                            item = list.GetRandomItem(ItemRank.흔함, false);
                            color = Color.green;
                        }
                        hex = UnityEngine.ColorUtility.ToHtmlStringRGB(color);
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
                        rand = UnityEngine.Random.Range(0, 1000);
                        if (rand < 35)
                        {
                            SetUpState(list.FindItem("함선", ItemRank.히든));
                            string hex = UnityEngine.ColorUtility.ToHtmlStringRGB(Color.skyBlue);
                            chat.Push($"<color=#{hex}>히든</color> 등급의 함선 획득.");
                            return;
                        }

                        Item item = list.GetRandomItem(ItemRank.특별함, false);

                        chat.Push($"중급 도박으로 <color=Yellow>{item.Rank}</color> 등급의 {item.Name} 획득.");
                    }
                    else
                        chat.Push("<color=red>획득에 실패하였습니다</color>");
                    break;
                case ItemRank.희귀함:
                    if (rand < 30)
                    {
                        SetUpState(list.FindItem("행운의 토큰", ItemRank.희귀함));
                        chat.Push("<color=red>획득에 실패하여 행운의 토큰을 얻습니다</color>");
                    }
                    else
                    {
                        if (UnityEngine.Random.Range(0, 100) < 4)
                        {
                            SetUpState(list.FindItem("이브", ItemRank.히든));
                            string hex = UnityEngine.ColorUtility.ToHtmlStringRGB(Color.skyBlue);
                            chat.Push($"<color=#{hex}>히든</color> 등급의 이브 획득.");
                        }
                        else
                        {
                            if (UnityEngine.Random.Range(0, 100) < 50)
                            {
                                Item item = list.GetRandomItem(ItemRank.희귀함, false);

                                chat.Push($"고급 도박으로 <color=#FF00FF>{item.Rank}</color> 등급의 {item.Name} 획득.");
                            }
                            else
                            {
                                Item item = list.GetRandomItem(ItemRank.특별함, false);

                                chat.Push($"고급 도박으로 <color=Yellow>{item.Rank}</color> 등급의 {item.Name} 획득.");
                            }
                        }
                    }
                    break;
            }
            GameManager.Instance.scrollView.ImageInit(list.currentItem[GameManager.Instance.Action.targetNumber]);
            list.FindItem("기억 조각", ItemRank.All).count -= neccesary;
            Clear(editItem, false);
        }
        else chat.Push("기억 조각이 부족합니다");

    }

    private void SetUpState(Item item)
    {
        item.count++;
        if(item.count == 1) list.GotItem.Enqueue(item);
    }

    public Image[] GetImages() { return images; }
    public Button[] GetButtons() { return buttons; }
    public UnityEngine.UI.Outline[] GetOutlines() { return outlines; }

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
                    transform.Find($"{str}/상위"),
                    transform.Find($"{str}/획득"),
                };

            foreach (Transform monoMenu in rankMenu)
            {
                monoMenu.GetComponent<UnityEngine.UI.Outline>().effectDistance = Vector2.zero;
            }

            rankMenu[rank].GetComponent<UnityEngine.UI.Outline>().effectDistance = new Vector2(4, 4);

            if (rank < (int)ItemRank.상위)
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
                    Dictionary<(string, ItemRank), int> dict = list.CombineAllItem(Items[i], false);
                    int all = 0;
                    foreach (KeyValuePair<(string, ItemRank), int> kvp in dict)
                    {
                        all += Mathf.Max(0, kvp.Value - list.FindItem(kvp.Key.Item1, kvp.Key.Item2).count);
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

                    for (int j = 0; j < list.itemList[0].Count; j++)
                    {
                        images[i + j].transform.Find("number1").gameObject.SetActive(true);
                        images[i + j].sprite = Resources.Load<Sprite>($"Image/Item/All/{list.itemList[0][j].Name}");
                        images[i + j].GetComponentInChildren<TextMeshProUGUI>().text = list.itemList[0][j].count.ToString();

                        Color c = images[i + j].color;
                        if (list.itemList[0][j].count == 0) c.a = blur;
                        else c.a = 1f;
                        images[i + j].color = c;
                    }
                }
                else if (rank == 2 && willBeGet != -1)
                {
                    UnityEngine.UI.Outline outline = buttons[willBeGet].GetComponent<UnityEngine.UI.Outline>();
                    outline.effectColor = Color.yellow;
                    outline.effectDistance = new Vector2(4, 4);
                }

            }
            else
            {
                PriorityQueue<Item> items = list.GotItem;

                int i = 0;
                foreach (Item it in items.EnumerateByPriority())
                {
                    images[i].sprite = it.Resource;
                    outlines[i].effectDistance = new Vector2(4, 4);
                    outlines[i].effectColor = GetColor(it);

                    images[i].transform.Find("number1").gameObject.SetActive(true);
                    TextMeshProUGUI countText = images[i].GetComponentInChildren<TextMeshProUGUI>();
                    countText.text = it.count.ToString();

                    i++;
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

                UnityEngine.UI.Outline line = buttons[i].GetComponent<UnityEngine.UI.Outline>();

                line.effectColor = GetColor(parentItems[i]);

                line.effectDistance = new Vector2(4, 4);
            }

            images[10 * 2].sprite = item.Resource;
            UnityEngine.UI.Outline targetItemLine = buttons[10 * 2].GetComponent<UnityEngine.UI.Outline>();
            Dictionary<(string, ItemRank), int> Colordict = list.CombineAllItem(targetItem, false);

            int all = 0;
            foreach (KeyValuePair<(string, ItemRank), int> kvp in Colordict)
            {
                all += Mathf.Max(0, kvp.Value - list.FindItem(kvp.Key.Item1, kvp.Key.Item2).count);
            }

            if (all != 0)
            {
                if (all > list.FindItem("만물석", ItemRank.All).count) targetItemLine.effectColor = Color.red;
                else targetItemLine.effectColor = Color.orange;
            }
            else targetItemLine.effectColor = Color.blue;
            targetItemLine.effectDistance = new Vector2(4f, 4f);
            images[10 * 2 + 1].sprite = Resources.Load<Sprite>($"Image/등호");

            ItemIngredient[] ingredient = item.NecessaryItem;
            for (int i = 0; i < ingredient.Length; i++)
            {
                images[10 * 2 + 1 + 1 + i].sprite = ingredient[i].Item.Resource;
                targetItemLine = buttons[10 * 2 + 1 + 1 + i].GetComponent<UnityEngine.UI.Outline>();
                targetItemLine.effectDistance = new Vector2(4f, 4f);


                images[10 * 2 + 1 + 1 + i].transform.Find("number1").gameObject.SetActive(true);

                images[10 * 2 + 1 + 1 + i].GetComponentInChildren<TextMeshProUGUI>().text = ingredient[i].Count.ToString();

                targetItemLine.effectColor = GetColor(ingredient[i].Item);
            }
            if (item.Rank != 0)
            {
                Dictionary<(string, ItemRank), int> dict = list.DissolutionAll(targetItem);

                object[,] common = list.table[(int)ItemRank.흔함];
                string[] names = Enumerable.Range(0, common.GetLength(0))   // 모든 행 인덱스
                                .Select(i => (string)common[i, 0])
                                .ToArray();
                int j = 0;
                foreach (string name in names)
                {
                    Transform tr = images[10 * 5 + j++].transform.Find("number1");
                    tr.gameObject.SetActive(true);
                    TextMeshProUGUI countText = tr.GetComponentInChildren<TextMeshProUGUI>();
                    if (dict.ContainsKey((name, ItemRank.흔함)))
                    {
                        countText.text = dict[(name, ItemRank.흔함)].ToString();
                    }
                    else
                        countText.text = "0";
                }
                j = 0;
                all = 0;
                foreach (string name in names)
                {
                    targetItem = list.FindItem(name, ItemRank.흔함);
                    images[10 * 5 + j].sprite = targetItem.Resource;
                    Transform tr = images[10 * 5 + j++].transform.Find("number2");
                    tr.gameObject.SetActive(true);
                    TextMeshProUGUI countText = tr.GetComponentInChildren<TextMeshProUGUI>();
                    if (Colordict.ContainsKey((name, ItemRank.흔함)))
                    {
                        int neccesary = Mathf.Max(Colordict[(name, ItemRank.흔함)] - list.FindItem(name, ItemRank.흔함).count, 0);
                        countText.text = neccesary.ToString();
                        all += neccesary;
                    }
                    else countText.text = "0";
                }
                foreach (KeyValuePair<(string, ItemRank), int> kvp in Colordict)
                {
                    Debug.Log($"{kvp.Key.Item1}, {kvp.Key.Item2}, {kvp.Value}");
                }

                images[10 * 5 + j].sprite = list.FindItem("만물석", ItemRank.All).Resource;
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

    public Color GetColor(Item targetItem)
    {
        switch (targetItem.Rank)
        {
            case ItemRank.All:
                return Color.skyBlue;
            case ItemRank.흔함:
                return Color.green;
            case ItemRank.안흔함:
                return new Color32(176, 78, 248, 255);
            case ItemRank.특별함:
                return Color.yellow;
            case ItemRank.희귀함:
                return new Color32(255, 0, 255, 255);
            case ItemRank.전설적인:
                return Color.red;
            case ItemRank.히든:
                return new Color32(233, 119, 157, 255);
            case ItemRank.변화된:
                return new Color32(255, 0, 131, 255);
            case ItemRank.상위:
                return new Color32(0, 248, 153, 255);
            default:
                return Color.skyBlue;
        }

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
            ItemStatus[7].text = $"체력 재생 : {DataManager.Instance.RoundX(editItem.HealthRegen,3)}";
            ItemStatus[8].text = $"마나 재생 : {DataManager.Instance.RoundX(editItem.ManaRegen , 3)}";
            ItemStatus[9].text = $"이동속도 감소 : {editItem.MoveSpeed}";
            ItemStatus[10].text = $"공격속도 증가 : {editItem.AttackSpeed}%";
            ItemStatus[11].text = $"타워 공격력 증가 : {editItem.TowerDamage}";
            ItemStatus[12].text = $"타워 공격속도 증가 : {editItem.TowerAttackSpeed}%";
            ItemStatus[13].text = $"공격 유형 : {editItem.AttackType}";

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
            int boss = editItem.BossAttack;

            if (Percentage != 0)
            {
                s.AppendLine($"스킬 확률 : {Percentage}%");
                if (MonoPhysics != 0) s.AppendLine($"단일 물리 데미지 : {MonoPhysics}");
                if (MultiPhysics != 0) s.AppendLine($"범위 물리 데미지 : {MultiPhysics}");
                if (MonoMagic != 0) s.AppendLine($"단일 마법 데미지 : {MonoMagic}");
                if (MultiMagic != 0) s.AppendLine($"범위 마법 데미지 : {MultiMagic}");
                if (MonoStun != 0) s.AppendLine($"단일 스턴 : {MonoStun}초");
                if (MultiStun != 0) s.AppendLine($"범위 스턴 : {MultiStun}초");
                if (Range != 0) s.AppendLine($"스킬 범위 : {Range * 100}");
                if (MonoPercent != 0 && boss == 0) s.AppendLine($"단일 현재체력 비례 데미지 : {MonoPercent}%");
                if (EndPercent != 0 && boss == 0) s.AppendLine($"단일 전체체력 비례 데미지 : {EndPercent}%");
                if (MaxPercent != 0) s.AppendLine($"범위 전체체력 비례 데미지 : {MaxPercent}%");
                if (CurrPercent != 0) s.AppendLine($"범위 현재체력 비례 데미지 : {CurrPercent}%");
                if (Max_CurrPercent != 0) s.AppendLine($"범위 잃은체력 비례 데미지 : {Max_CurrPercent}%");
                if (boss != 1) s.AppendLine($"보스 대상 현재체력 비례 데미지 : {MonoPercent}%"); 
                if (boss != 2) s.AppendLine($"보스 대상 전체체력 비례 데미지 : {EndPercent}%"); 

                switch ((editItem.Name, editItem.Rank))
                {
                    case ("좀비", ItemRank.안흔함):
                        s.AppendLine($"스킬 발동시 적 유닛이 사망하면 좀비 아이템 1개 추가");
                        break;
                }

            }
            else
                s.AppendLine("스킬이 없습니다");


            ItemSkillExplanation.text = s.ToString();

        }
    }
}
