using System;
using System.Collections;
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
    public GameObject ItemList;
    public GameObject editItemStatus;
    public Image statusItem;
    public TextMeshProUGUI editItemName;
    public TextMeshProUGUI[] ItemStatus;
    public TextMeshProUGUI ItemSkillExplanation;
    public ChatManager chat;

    public GameObject Count;
    [NonSerialized] public bool isAllToggle = true;

    [NonSerialized] public float SetSoulParts = 1;
    public int RerollCount = 2;

    public int willBeGet = -1;
    public GameObject targetImage;

    public Stack<Item> itemStack = new Stack<Item>();
    void Update()
    {
        if(!GameManager.Instance.items.activeSelf)
            return;
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (itemStack.Count == 0) Clear(null, true);
            else Clear(itemStack.Pop(), true);
            return;
        }

        // 2) Ctrl 상태 캐시
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        // 3) 키별로 Ctrl 조합/일반 동작 분기
        if (Input.GetKeyDown(KeyCode.Q)) { if (ctrl) ControlTrigger((int)DataManager.Num.Q); else if (!Count.activeInHierarchy)TriggerMany((int)DataManager.Num.Q,1); return; }
        if (Input.GetKeyDown(KeyCode.W)) { if (ctrl) ControlTrigger((int)DataManager.Num.W); else if (!Count.activeInHierarchy)TriggerMany((int)DataManager.Num.W,1); return; }
        if (Input.GetKeyDown(KeyCode.E)) { if (ctrl) ControlTrigger((int)DataManager.Num.E); else if (!Count.activeInHierarchy)TriggerMany((int)DataManager.Num.E,1); return; }
        if (Input.GetKeyDown(KeyCode.D)) { if (!Count.activeInHierarchy) TriggerMany((int)DataManager.Num.D, 1); return; }
        if (Input.GetKeyDown(KeyCode.Z)) { if (ctrl) ControlTrigger((int)DataManager.Num.Z); else if (!Count.activeInHierarchy)TriggerMany((int)DataManager.Num.Z,1); return; }
        if (Input.GetKeyDown(KeyCode.X)) { if (ctrl) ControlTrigger((int)DataManager.Num.X); else if (!Count.activeInHierarchy)TriggerMany((int)DataManager.Num.X,1); return; }
        if (Input.GetKeyDown(KeyCode.C)) { if (ctrl) ControlTrigger((int)DataManager.Num.C); else if (!Count.activeInHierarchy)TriggerMany((int)DataManager.Num.C,1); return; }
        if (Input.GetKeyDown(KeyCode.F) && rank == (int)ItemRank.희귀함 - 1 && editItem == null && targetImage != null)
        {
            Item targetItem = list.FindItem(targetImage.transform.Find("Image").GetComponent<Image>().sprite.name, ItemRank.희귀함);
            if (targetItem.count > 0)
            {
                if (RerollCount > 0)
                {
                    targetItem.count--;
                    list.ChangeItem(targetItem);
                    Clear(editItem, false);
                }           
                else
                    chat.Push("횟수가 부족하여 리롤을 할 수 없습니다");
            }


        }
        
    }

    public void SetList()
    {
        images = ItemList.GetComponentsInChildren<Image>().Where(img => img.gameObject.name.ToLower().Contains("image")).ToArray();
        buttons = ItemList.GetComponentsInChildren<Button>().Where(img => img.gameObject.name.ToLower().Contains("button")).ToArray();
        outlines = buttons
            .Select(b => b.GetComponent<UnityEngine.UI.Outline>())
            .Where(o => o != null)
            .ToArray();

                Count = GameManager.Instance.Count;
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
                numberImage.raycastTarget = false;

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
                text.raycastTarget = false;

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

        menu = GetComponent<Transform>().Find("Items/row1").GetComponentsInChildren<Image>().Where(img =>
        !img.gameObject.name.ToLower().Contains("row")).ToArray();

        foreach (Image image in menu)
        {
            image.AddComponent<MyButtonTrigger>();
        }
        list = new List(GameManager.Instance.playerStats[DataManager.targetNumberMax - 1], GameManager.Instance.cannonManager, this);
        list.Clear();
    }

    public void TriggerMany(int rank, int amount)
    {
        StartCoroutine(Trigger(rank, amount));
    }
    private IEnumerator Trigger(int rank, int amount)
    {
        for(int i=0;i<amount;i++)
        switch ((DataManager.Num)rank)
        {
            case DataManager.Num.Q:
                GetRankedItem(ItemRank.흔함);
                yield return null;
                break;
            case DataManager.Num.W:
                GetRankedItem(ItemRank.특별함);
                yield return null;
                break;
            case DataManager.Num.E:
                GetRankedItem(ItemRank.희귀함);
                yield return null;
                break;
            case DataManager.Num.Z:
            case DataManager.Num.X:
            case DataManager.Num.C:
                if (isAllToggle)
                {
                    SetSoulParts = 1 << (rank - (int)DataManager.Num.Z);
                    SetSouls(true);
                }
                else
                {
                    Item item = list.FindItem("영혼 파편", ItemRank.All);
                    if (item.count > 0)
                    {
                        item.count -= 1;
                        switch ((DataManager.Num)rank)
                        {
                            case DataManager.Num.Z:
                                list.GetRandomItem(ItemRank.흔함);
                                break;
                            case DataManager.Num.X:
                                int rand = UnityEngine.Random.Range(0, 100);
                                if (rand < 66)
                                    list.GetMemoriesParts(1);
                                else
                                    chat.Push($"<color=Yellow>기억 조각</color> 획득에 실패했습니다.");
                                break;
                            case DataManager.Num.C:
                                list.GetSoulMana(1);
                                break;
                                
                        }
                    }
                    else
                    {
                        chat.Push($"영혼 파편이 없습니다.");
                    }
                }
                yield return null;
                break;
            case DataManager.Num.D:
                isAllToggle = !isAllToggle;
                if(isAllToggle == true ) list.ChangeSouls();
                GameManager.Instance.Images[(int)DataManager.Num.D].GetComponent<UnityEngine.UI.Outline>().enabled = isAllToggle;
                SetSouls(isAllToggle);
                yield return null;
                break;
        }
        yield return null;
    }

    public void ControlTrigger(int rank)
    {
        CountScript script = Count.GetComponent<CountScript>();

        script.SetNumber(rank);
        GameManager.Instance.SetCountScript();
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
        Item item = null;
        if (rank == ItemRank.흔함) neccesary = 1;
        if (rank == ItemRank.안흔함) neccesary = 1;
        else if (rank == ItemRank.특별함) neccesary = 2;
        else if (rank == ItemRank.희귀함) neccesary = 4;

        if (list.FindItem("기억 조각", ItemRank.All).count >= neccesary)
        {
            int rand = UnityEngine.Random.Range(0, 100);
            Color color;
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
                        rand = UnityEngine.Random.Range(0, 100);

                        if (rand < 50)
                        {
                            item = list.GetRandomItem(ItemRank.안흔함, false);
                            color = new Color32(176, 78, 248, 255);
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

                        item = list.GetRandomItem(ItemRank.특별함, false, false);

                        chat.Push($"중급 도박으로 <color=Yellow>{item.Rank}</color> 등급의 {item.Name} 획득.");
                    }
                    else
                        chat.Push("<color=red>획득에 실패하였습니다</color>");
                    break;
                case ItemRank.희귀함:
                    if (rand < 30)
                    {
                        item = list.FindItem("행운의 토큰", ItemRank.희귀함);
                        item.count++;
                         Clear(editItem, false);
                        chat.Push("<color=red>획득에 실패하여 행운의 토큰을 얻습니다</color>");
                    }
                    else
                    {
                        {
                            if (UnityEngine.Random.Range(0, 100) < 50)
                            {
                                item = list.GetRandomItem(ItemRank.희귀함, false, true);
                            }
                            else
                            {
                                item = list.GetRandomItem(ItemRank.특별함, false, true);
                            }
                        }
                        Color color1 = GetColor(item);
                        chat.Push($"고급 도박으로 <color=#{UnityEngine.ColorUtility.ToHtmlStringRGBA(color1)}>{item.Rank}</color> 등급의 {item.Name} 획득");
                    }
                    break;
                    default:
                    chat.Push("알 수 없는 아이템 명령어");
                    break;
            }
            list.FindItem("기억 조각", ItemRank.All).count -= neccesary;
        }
        else chat.Push("기억 조각이 부족합니다");

    }

    public void SetUpState(Item item)
    {
        item.count++;
        if (item.count == 1)
        {
            list.GotItem.Enqueue(item);
            if(item.MoveSpeed != 0 || item.NeutralizeDefense != 0)
                list.DebuffItem.Enqueue(item);
            list.SetUnity(item);
            if(list.currentItem == null)Debug.Log("current item 없음");
            if(GameManager.Instance.originStatFor6 == null) Debug.Log("originstatfor6 없음");
            GameManager.Instance.scrollView.ImageInit(list.currentItem[GameManager.Instance.originStatFor6.targetNumber], true);
        }
        else
            GameManager.Instance.scrollView.ImageInit(list.currentItem[GameManager.Instance.originStatFor6.targetNumber], false);
        Clear(editItem, false);
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
        if(item != null && item.Rank == 0) return;
        if(!gameObject.activeSelf) return;
        editItem = item;
        list.Clear();

        if (ClearStatus)
        {
            editItemStatus.SetActive(false);
            ItemList.SetActive(true);
        }

        if(!ItemList.activeSelf) return;

        if (item == null) // 아이템 누르기 전 메뉴창
        {
            string str = "Items/row1";

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
                    Dictionary<(string, ItemRank), int> dict = list.CombineAllItem(Items[i]);
                    int all = 0;
                    foreach (KeyValuePair<(string, ItemRank), int> kvp in dict)
                    {
                        if(kvp.Key.Item2 == ItemRank.흔함)
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

        else // 아이템 눌렀을 때
        {
            Item targetItem = item;
            List<Item> parentItems = targetItem.GetParent();
            int all = 0;
            for (int i = 0; i < parentItems.Count; i++)
            {
                images[i].sprite = parentItems[i].Resource;

                UnityEngine.UI.Outline line = buttons[i].GetComponent<UnityEngine.UI.Outline>();

                line.effectColor = GetColor(parentItems[i]);

                line.effectDistance = new Vector2(4, 4);

                GameObject gameObject = images[i].transform.Find("number2").gameObject;

                gameObject.SetActive(true);

                
                Dictionary<(string, ItemRank), int> dict = list.CombineAllItem(parentItems[i]);
                all = 0;
                foreach (KeyValuePair<(string, ItemRank), int> kvp in dict)
                {
                    if(kvp.Key.Item2 == ItemRank.흔함)
                        all += Mathf.Max(0, kvp.Value - list.FindItem(kvp.Key.Item1, kvp.Key.Item2).count);
                }
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = all.ToString();
            }

            images[10 * 2].sprite = item.Resource;
            UnityEngine.UI.Outline targetItemLine = buttons[10 * 2].GetComponent<UnityEngine.UI.Outline>();
            Dictionary<(string, ItemRank), int> Colordict = list.CombineAllItem(targetItem);

            all = 0;
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
                images[10 * 2 + 1 + 1 + i].sprite = list.FindItem(ingredient[i].ItemName, ingredient[i].Rank).Resource;
                targetItemLine = buttons[10 * 2 + 1 + 1 + i].GetComponent<UnityEngine.UI.Outline>();
                targetItemLine.effectDistance = new Vector2(4f, 4f);


                images[10 * 2 + 1 + 1 + i].transform.Find("number1").gameObject.SetActive(true);

                images[10 * 2 + 1 + 1 + i].GetComponentInChildren<TextMeshProUGUI>().text = ingredient[i].Count.ToString();

                targetItemLine.effectColor = GetColor(list.FindItem(ingredient[i].ItemName, ingredient[i].Rank));
            }
            if (item.Rank != 0)
            {
                Dictionary<(string, ItemRank), int> dict = list.DissolutionAll(targetItem);

                List<ItemDef> common = list.table[(int)ItemRank.흔함];
                string[] names = Enumerable.Range(0, common.Count)   // 모든 행 인덱스
                                .Select(i => common[i].Name)
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

    public void FakeClear(Item item)
    {
        if(editItem != null)return;
        if(rank != (int)item.Rank) return;
    }

    public  Item GetEditItem() { return  editItem; }

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
}
